using System;
using System.Threading;
using System.Threading.Tasks;

namespace Snipper.Templates;

/// <summary>
/// Represents a factory that creates templates.
/// </summary>
public interface ITemplateFactory
{
    /// <summary>
    /// Gets the human-readable name that identifies the type of template produced by this factory.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Asynchronously creates a new instance of this template.
    /// </summary>
    /// <param name="settings">
    /// The settings associated with the instance.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token that controls the lifetime of the operation.
    /// </param>
    /// <returns>
    /// The instance that was created.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// Thrown when the operation aborts because <paramref name="cancellationToken"/> is cancelled.
    /// </exception>
    Task<ITemplate> CreateAsync(TemplateSettings settings, CancellationToken cancellationToken);
}