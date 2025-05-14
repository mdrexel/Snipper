using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using Snipper.Files;

namespace Snipper.Templates.Images;

/// <summary>
/// An image template snipper factory implementation.
/// </summary>
public sealed class ImageTemplateFactory : ITemplateFactory
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageTemplateFactory"/> class.
    /// </summary>
    public ImageTemplateFactory()
    {
    }

    /// <inheritdoc/>
    public bool TryCreate(
        TemplateSettings settings,
        [NotNullWhen(true)] out ITemplate? template)
    {
        if (settings is null)
        {
            template = default;
            return false;
        }

        List<Segment> segments = [];
        List<AbsoluteFilePath> files = [];
        foreach (AbsolutePath path in settings.Paths)
        {
            if (!AbsoluteFilePath.TryFromExisting(path.Value, out AbsoluteFilePath? file))
            {
                // All specified paths must be existing files.
                template = default;
                return false;
            }

            if (file.Extension == FileExtension.Json)
            {
                IReadOnlyList<Segment>? segment = null;
                try
                {
                    using FileStream stream = File.OpenRead(path.Value);
                    segment = JsonSerializer.Deserialize(stream, SegmentContext.Default.IReadOnlyListSegment);
                }
                catch
                {
                    // Swallow any exceptions.
                }

                if (segment is null)
                {
                    // All JSON files must be able to be parsed as segments.
                    template = default;
                    return false;
                }

                segments.AddRange(segment);
            }
            else
            {
                files.Add(file);
            }
        }

        bool result = ImageTemplate.TryCreate(segments, files, out ImageTemplate? buffer);
        template = buffer;
        return result;
    }
}