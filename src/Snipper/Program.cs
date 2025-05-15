using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using Snipper.Files;
using Snipper.Templates;
using Snipper.Templates.Images;
using Windows.Win32;
using Windows.Win32.Storage.FileSystem;
using Windows.Win32.System.Console;

namespace Snipper;

/// <summary>
/// The entry point for the application.
/// </summary>
internal sealed class Program
{
    private static IReadOnlyList<ITemplateFactory> TemplateFactories { get; } =
        [
            new ImageTemplateFactory(),
        ];

    /// <summary>
    /// The entry point for the application.
    /// </summary>
    /// <param name="args">
    /// The arguments associated with the application invocation.
    /// </param>
    /// <returns>
    /// The application exit code.
    /// </returns>
    public static async Task<int> Main(string[] args)
    {
        // Sanity-check: did we actually receive any arguments? (They may have just double-clicked on the `.exe`)
        if (args.Length == 0)
        {
            Console.WriteLine(
                "No files were specified as command-line arguments. You can run this program from the command-line, or drag-and-drop your files onto the executable.");
            return Exit(ExitCode.NoFiles);
        }

        using CancellationTokenSource cts = new();
        Console.CancelKeyPress +=
            (obj, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

        int exitCode;
        try
        {
            exitCode = await RunAsync(args, cts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            if (cts.IsCancellationRequested)
            {
                // We triggered this.
                return Exit(ExitCode.Cancelled);
            }

            // We have no idea what's happening, explode.
            throw;
        }

        return Exit(exitCode);
    }

    internal static async Task<int> RunAsync(IReadOnlyList<string> args, CancellationToken cancellationToken)
    {
        IReadOnlyList<AbsolutePath> paths = args.Select(x => new AbsolutePath(x)).ToArray();
        TemplateSettings settings = new(paths);

        ITemplate? template = null;
        foreach (ITemplateFactory factory in TemplateFactories)
        {
            try
            {
                template = await factory.CreateAsync(settings, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                template = null;
                throw;
            }
            catch (Exception e)
            {
                template = null;

                // Assume the failure is because `settings` is not valid for this factory, and that the exception
                // describes the issue. (If we're wrong we just swallowed an important exception, oops!)
                Console.WriteLine(e);
            }

            break;
        }

        if (template is null)
        {
            Console.WriteLine("No registered template can handle the specified files.");
            return ExitCode.NoTemplate;
        }

        await template.ExecuteAsync(cancellationToken).ConfigureAwait(false);
        return ExitCode.Success;
    }

    /// <summary>
    /// Returns <paramref name="exitCode"/> after waiting for the human to acknowledge any non-success status code.
    /// </summary>
    /// <param name="exitCode">
    /// The exit code.
    /// </param>
    /// <returns>
    /// <paramref name="exitCode"/>, after the human has acknowledged any non-success status code.
    /// </returns>
    internal static int Exit(int exitCode)
    {
        // Did we successfully snip their files? If so, we have nothing useful to report to the human.
        if (exitCode == ExitCode.Success)
        {
            return exitCode;
        }

        // We write output to the console, but the console might close before the human has time to read it.
        // A console is destroyed when all attached processes have exited.
        // By checking the number of attached processes, we can determine whether we're the last attached process.
        // 
        // The array we're passing in gets filled with the process identifiers that are attached to the console. We
        // don't actually care about those, we just need to know how many there are, which is what the return value
        // tells us.
        uint count = PInvoke.GetConsoleProcessList(new uint[2]);
        if (count > 1)
        {
            // Someone other than us is keeping the console alive - assume it's safe to exit.
            return exitCode;
        }

        // We know we're the last process attached to the console... but has this console had its output redirected? If
        // so, the caller probably intends for us to exit like normal rather than block and wait for user input.
        using SafeFileHandle foo = PInvoke.GetStdHandle_SafeHandle(STD_HANDLE.STD_OUTPUT_HANDLE);
        if (Marshal.GetLastWin32Error() == -1)
        {
            // If we were unable to retrieve the stdout handle, assume something funny is going on and we should exit.
            return exitCode;
        }

        if (PInvoke.GetFileInformationByHandle(foo, out BY_HANDLE_FILE_INFORMATION lpFileInformation))
        {
            // If our stdout handle has file information, it means we've been redirected.
            return exitCode;
        }

        // There's no-one else keeping the console alive, and we're printing to the console. Give the human a chance to
        // read the output.
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey(intercept: true);

        return exitCode;
    }

    private static class ExitCode
    {
        public const int Success = 0;
        public const int NoTemplate = 1;
        public const int Cancelled = 2;
        public const int NoFiles = 3;
    }
}