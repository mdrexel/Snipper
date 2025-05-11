using System.Diagnostics.CodeAnalysis;

namespace Snipper.Templates;

/// <summary>
/// Represents a factory that creates templates.
/// </summary>
public interface ITemplateFactory
{
    /// <summary>
    /// Tries to initialize a new instance of this template type.
    /// </summary>
    /// <param name="settings">
    /// The settings associated with the instance.
    /// </param>
    /// <param name="template">
    /// When this method returns <see langword="true"/>, set to the instance that was initialized; otherwise, set to
    /// <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the specified <paramref name="settings"/> were understood by this template type;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    bool TryCreate(
        TemplateSettings settings,
        [NotNullWhen(returnValue: true)] out ITemplate? template);
}