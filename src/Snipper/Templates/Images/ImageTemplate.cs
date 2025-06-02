using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Snipper.Files;
using Snipper.Templates.Images.Models;

namespace Snipper.Templates.Images;

/// <summary>
/// An image template snipper implementation.
/// </summary>
public sealed class ImageTemplate : ITemplate
{
    private ImageTemplate(
        IReadOnlyList<Segment> segments,
        IReadOnlyList<AbsoluteFilePath> files)
    {
        Segments = segments;
        Files = files;
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
    /// Asynchronously initializes a new instance of the <see cref="ImageTemplate"/> class.
    /// </summary>
    /// <param name="segments">
    /// The segments to snip from the specified <paramref name="files"/>.
    /// </param>
    /// <param name="files">
    /// The files to snip from.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token controlling the lifetime of the operation.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="segments"/> or <paramref name="files"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when any of the following is true:
    /// <list type="bullet">
    ///   <item><paramref name="segments"/> contains <see langword="null"/>.</item>
    ///   <item><paramref name="segments"/> is empty.</item>
    ///   <item><paramref name="segments"/> contains segments with duplicate case-insensitive names.</item>
    ///   <item><paramref name="files"/> contains <see langword="null"/>.</item>
    ///   <item><paramref name="files"/> contains items with a <see cref="AbsoluteFilePath.Extension"/> of <see langword="null"/>.</item>
    ///   <item><paramref name="files"/> contains items with an unsupported <see cref="AbsoluteFilePath.Extension"/>.</item>
    /// </list>
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// Thrown when the operation aborts because <paramref name="cancellationToken"/> is cancelled.
    /// </exception>
    public static async Task<ImageTemplate> CreateAsync(
        IReadOnlyList<Segment> segments,
        IReadOnlyList<AbsoluteFilePath> files,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        segments.ThrowIfNull(nameof(segments)).ThrowIfContainsNull(nameof(segments));
        if (!segments.Any())
        {
            throw new ArgumentException(
                "The specified segment collection is empty. At least one segment must be specified.",
                nameof(segments));
        }

        IReadOnlyList<string> duplicates = GetDuplicateNames(segments);
        if (duplicates.Any())
        {
            string duplicateNames = string.Join(", ", duplicates);
            throw new ArgumentException(
                $"The specified segment collection contains duplicate names. Duplicate names: {duplicateNames}",
                nameof(segments));
        }

        files.ThrowIfNull(nameof(files)).ThrowIfContainsNull(nameof(files));
        IReadOnlyList<AbsoluteFilePath> missing = files.Where(x => x.Extension is null).ToArray();
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

        return new ImageTemplate(segments, files);
    }

    /// <inheritdoc/>
    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        foreach (AbsoluteFilePath file in Files)
        {
            await SnipAsync(file, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Snips the specified <paramref name="inputPath"/> using the segments associated with this instance.
    /// </summary>
    /// <param name="inputPath">
    /// The file to snip.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token controlling the lifetime of the operation.
    /// </param>
    /// <returns>
    /// A task representing the operation.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// Thrown when the operation aborts because <paramref name="cancellationToken"/> is cancelled.
    /// </exception>
    private async Task SnipAsync(AbsoluteFilePath inputPath, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using Image image = Image.FromFile(inputPath.Value);
        foreach (Segment segment in Segments)
        {
            await ImageTemplate
                .SnipAsync(
                    (inputPath, image),
                    new Region()
                    {
                        Name = segment.Name,
                        X = segment.Region.X,
                        Y = segment.Region.Y,
                        Height = segment.Region.Height,
                        Width = segment.Region.Width,
                    },
                    new Scaling()
                    {
                        Factor = checked((int)(segment.Scaling?.Factor ?? 1U)),
                        Mode = segment.Scaling?.Mode is not null
                            ? Convert(segment.Scaling.Mode.Value)
                            : System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor,
                    },
                    cancellationToken)
                .ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Returns an output file path using the specified <paramref name="inputFile"/>, suffixing the specified
    /// <paramref name="name"/> (and a disambiguating number if necessary).
    /// </summary>
    /// <param name="inputFile">
    /// The input file path.
    /// </param>
    /// <param name="name">
    /// The name associated with the output file.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token controlling the lifetime of the operation.
    /// </param>
    /// <returns>
    /// An output file path.
    /// </returns>
    /// <exception cref="OperationCanceledException">
    /// Thrown when the operation aborts because <paramref name="cancellationToken"/> is cancelled.
    /// </exception>
    private static async Task<string> GetOutputPathAsync(
        AbsoluteFilePath inputFile,
        string name,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        int counter = 1;
        string output = $"{inputFile.WithoutExtension}.{name}.{inputFile.Extension}";
        while (File.Exists(output))
        {
            cancellationToken.ThrowIfCancellationRequested();

            output = $"{inputFile.WithoutExtension}.{name} ({counter++}).{inputFile.Extension}";
        }

        return output;
    }

    private static async Task SnipAsync(
        (AbsoluteFilePath Path, Image Image) input,
        Region region,
        Scaling scaling,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        int width = checked(region.Width * scaling.Factor);
        int height = checked(region.Height * scaling.Factor);

        using Bitmap buffer = new(width, height, input.Image.PixelFormat);
        using Graphics graphics = Graphics.FromImage(buffer);
        graphics.InterpolationMode = scaling.Mode;
        graphics.CompositingQuality = CompositingQuality.HighQuality;
        graphics.PixelOffsetMode = PixelOffsetMode.Half;

        graphics.DrawImage(
            input.Image,
            new Rectangle(
                0,
                0,
                width,
                height),
            new Rectangle(
                region.X,
                region.Y,
                region.Width,
                region.Height),
            GraphicsUnit.Pixel);

        string outputPath = await ImageTemplate
            .GetOutputPathAsync(
                input.Path,
                region.Name,
                cancellationToken)
            .ConfigureAwait(false);

        buffer.Save(outputPath, input.Image.RawFormat);
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

    private static System.Drawing.Drawing2D.InterpolationMode Convert(Models.InterpolationMode mode)
    {
        mode.ThrowIfNotDefined();

        return
            mode switch
            {
                Models.InterpolationMode.NearestNeighbour => System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor,
                Models.InterpolationMode.Bilinear => System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear,
                Models.InterpolationMode.Bicubic => System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic,
                _ => throw new NotSupportedException("The specified interpolation mode could not be mapped to a GDI+ interpolation mode."),
            };
    }

    private sealed class Region
    {
        public required string Name { get; init; }

        public required int X { get; init; }

        public required int Y { get; init; }

        public required int Height { get; init; }

        public required int Width { get; init; }
    }

    private sealed class Scaling
    {
        public required int Factor { get; init; }

        public required System.Drawing.Drawing2D.InterpolationMode Mode { get; init; }
    }
}