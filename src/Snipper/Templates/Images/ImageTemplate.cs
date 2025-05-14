using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
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
    /// Thrown when any of the following is true:
    /// <list type="bullet">
    ///   <item><paramref name="segments"/> contains <see langword="null"/>.</item>
    ///   <item><paramref name="files"/> contains <see langword="null"/>.</item>
    ///   <item><paramref name="segments"/> contains segments with duplicate case-insensitive names.</item>
    ///   <item><paramref name="files"/> contains items with a <see cref="AbsoluteFilePath.Extension"/> of <see langword="null"/>.</item>
    ///   <item><paramref name="files"/> contains items with an unsupported <see cref="AbsoluteFilePath.Extension"/>.</item>
    /// </list>
    /// </exception>
    public ImageTemplate(
        IReadOnlyList<Segment> segments,
        IReadOnlyList<AbsoluteFilePath> files)
    {
        Segments = segments
            .ThrowIfNull(nameof(segments))
            .ThrowIfContainsNull(nameof(segments));

        IReadOnlyList<string> duplicates = GetDuplicateNames(segments);
        if (duplicates.Any())
        {
            string duplicateNames = string.Join(", ", duplicates);
            throw new ArgumentException(
                $"The specified segment collection contains duplicate names. Duplicate names: {duplicateNames}",
                nameof(segments));
        }

        Files = files
            .ThrowIfNull(nameof(files))
            .ThrowIfContainsNull(nameof(files));
        IReadOnlyList<AbsoluteFilePath> missing = files
            .Where(x => x.Extension is null)
            .ToArray();
        if (missing.Count > 0)
        {
            string missingExtensions = string.Join(", ", missing);
            throw new ArgumentException(
                $"The specified file collection contains files with no file extension. Missing extensions: {missingExtensions}",
                nameof(files));
        }

        IReadOnlyList<string> unsupported = GetUnsupportedExtensions(files);
        if (unsupported.Any())
        {
            string unsupportedExtensions = string.Join(", ", unsupported);
            throw new ArgumentException(
                $"The specified file collection contains unsupported file extensions. Unsupported extensions: {unsupportedExtensions}",
                nameof(files));
        }
    }

    // TODO: I know that really we should be checking the files for the magic number in their header or whatever, but
    // this is good enough for now.
    /// <summary>
    /// Gets the set of supported image file extensions.
    /// </summary>
    public static IReadOnlySet<FileExtension> SupportedExtensions { get; } =
        new HashSet<FileExtension>
        {
            FileExtension.Bmp,
            FileExtension.Jpeg,
            FileExtension.Jpg,
            FileExtension.Png,
        }.AsReadOnly();

    /// <summary>
    /// Gets the segments to snip.
    /// </summary>
    public IReadOnlyList<Segment> Segments { get; }

    /// <summary>
    /// Gets the files to snip from.
    /// </summary>
    public IReadOnlyList<AbsoluteFilePath> Files { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageTemplate"/> class.
    /// </summary>
    /// <param name="segments">
    /// The segments to snip from the specified <paramref name="files"/>.
    /// </param>
    /// <param name="files">
    /// The files to snip from.
    /// </param>
    /// <param name="template">
    /// When this method returns <see langword="true"/>, set to the template that was initialized. Otherwise, set to
    /// <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="template"/> was successfully initialized; otherwise,
    /// <see langword="false"/>.
    /// </returns>
    public static bool TryCreate(
        IReadOnlyList<Segment> segments,
        IReadOnlyList<AbsoluteFilePath> files,
        [NotNullWhen(returnValue: true)] out ImageTemplate? template)
    {
        // TODO: We should probably duplicate the logic rather than swallow all exceptions, since the exception could
        // be something unrelated to us - ex. `OutOfMemoryException` - but it's fine for now. Probably.
        try
        {
            template = new(segments, files);
            return true;
        }
        catch
        {
            template = null;
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        foreach (AbsoluteFilePath file in Files)
        {
            using Image input = Image.FromFile(file.Value);
            foreach (Segment segment in Segments)
            {
                using Bitmap buffer = new(segment.Width, segment.Height, input.PixelFormat);
                using Graphics graphics = Graphics.FromImage(buffer);

                graphics.DrawImage(
                    input,
                    new Rectangle(
                        0,
                        0,
                        segment.Width,
                        segment.Height),
                    new Rectangle(
                        segment.X,
                        segment.Y,
                        segment.Width,
                        segment.Height),
                    GraphicsUnit.Pixel);

                string output = $"{file.WithoutExtension}.{segment.Name}.{file.Extension}";
                buffer.Save(output, input.RawFormat);
            }
        }
    }

    private static IReadOnlyList<string> GetDuplicateNames(IReadOnlyList<Segment> segments)
    {
        return Impl(segments).ToArray();

        static IEnumerable<string> Impl(IReadOnlyList<Segment> segments)
        {
            HashSet<string> visited = new(StringComparer.OrdinalIgnoreCase);
            for (int counter = 0; counter < segments.Count; counter++)
            {
                string current = segments[counter].Name;
                if (!visited.Add(current))
                {
                    yield return current;
                }
            }
        }
    }

    private static IReadOnlyList<string> GetUnsupportedExtensions(IReadOnlyList<AbsoluteFilePath> files)
    {
        return Impl(files).ToArray();

        static IEnumerable<string> Impl(IReadOnlyList<AbsoluteFilePath> files)
        {
            for (int counter = 0; counter < files.Count; counter++)
            {
                FileExtension? current = files[counter].Extension;
                if (current is null)
                {
                    // Any files with a null extension should have already been filtered out, since they don't count as
                    // "unsupported". (You can't not-support something that doesn't exist.)
                    continue;
                }
                else if (!SupportedExtensions.Contains(current))
                {
                    yield return current.Normalized;
                }
            }
        }
    }
}