using System;
using System.Collections.Generic;
using Snipper.Files;

namespace Snipper.Templates;

/// <summary>
/// Contains application settings shared across all template implementations.
/// </summary>
public sealed class TemplateSettings
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateSettings"/> class.
    /// </summary>
    /// <param name="paths">
    /// The input paths that were specified.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="paths"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="paths"/> contains <see langword="null"/>.
    /// </exception>
    public TemplateSettings(
        IReadOnlyList<AbsolutePath> paths)
    {
        Paths = paths
            .ThrowIfNull(nameof(paths))
            .ThrowIfContainsNull(nameof(paths));
    }

    /// <summary>
    /// Gets the input paths that were specified.
    /// </summary>
    public IReadOnlyList<AbsolutePath> Paths { get; }
}