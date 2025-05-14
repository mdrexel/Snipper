using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Snipper.Files;

namespace Snipper.Templates.Images;

/// <summary>
/// An image template snipper implementation.
/// </summary>
public sealed class ImageTemplate : ITemplate
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageTemplate"/> class.
    /// </summary>
    /// <param name="segments">
    /// The segments to snip from the specified <paramref name="files"/>.
    /// </param>
    /// <param name="files">
    /// The files to snip from.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="segments"/> or <paramref name="files"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="segments"/> or <paramref name="files"/> contains <see langword="null"/>.
    /// </exception>
    public ImageTemplate(
        IReadOnlyList<Segment> segments,
        IReadOnlyList<AbsoluteFilePath> files)
    {
        Segments = segments
            .ThrowIfNull(nameof(segments))
            .ThrowIfContainsNull(nameof(segments));
        Files = files
            .ThrowIfNull(nameof(files))
            .ThrowIfContainsNull(nameof(files));
    }

    /// <summary>
    /// Gets the segments to snip.
    /// </summary>
    public IReadOnlyList<Segment> Segments { get; }

    /// <summary>
    /// Gets the files to snip from.
    /// </summary>
    public IReadOnlyList<AbsoluteFilePath> Files { get; }

    /// <inheritdoc/>
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();


    }
}