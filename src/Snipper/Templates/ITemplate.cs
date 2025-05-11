using System.Threading;
using System.Threading.Tasks;

namespace Snipper.Templates;

/// <summary>
/// Represents a template that maps a set of input files to a set of output files.
/// </summary>
public interface ITemplate
{
    /// <summary>
    /// Executes the template instance, snipping based on the state of the template.
    /// </summary>
    /// <param name="cancellationToken">
    /// A cancellation token controlling the lifetime of the operation.
    /// </param>
    /// <returns>
    /// A task representing the execution state.
    /// </returns>
    Task ExecuteAsync(CancellationToken cancellationToken);
}