using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Snipper.Files;
using Snipper.Templates;

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
        IReadOnlyList<AbsolutePath> paths = args.Select(x => new AbsolutePath(x)).ToArray();
        TemplateSettings settings = new(paths);

        ITemplate? template = null;
        foreach (ITemplateFactory factory in TemplateFactories)
        {
            if (factory.TryCreate(settings, out template))
            {
                break;
            }
        }

        if (template is null)
        {
            Console.WriteLine("No registered template can handle the specified files.");
            return 1;
        }

        using CancellationTokenSource cts = new();
        Console.CancelKeyPress +=
            (obj, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

        await template.ExecuteAsync(cts.Token).ConfigureAwait(false);
        return cts.IsCancellationRequested ? 2 : 0;
    }
}