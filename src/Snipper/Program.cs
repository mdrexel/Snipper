using System;
using System.Threading.Tasks;

namespace Snipper;

/// <summary>
/// The entry point for the application.
/// </summary>
internal sealed class Program
{
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
        Console.WriteLine("Hello, World!");

        Console.ReadLine();

        return 0;
    }
}