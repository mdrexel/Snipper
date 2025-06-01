using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Snipper.Files;
using Snipper.Templates.Images.Models;

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
    public string Name => nameof(ImageTemplate);

    /// <inheritdoc/>
    public async Task<ITemplate> CreateAsync(
        TemplateSettings settings,
        CancellationToken cancellationToken)
    {
        settings.ThrowIfNull();
        cancellationToken.ThrowIfCancellationRequested();

        List<Segment> segments = [];
        List<AbsoluteFilePath> files = [];
        foreach (AbsolutePath path in settings.Paths)
        {
            AbsoluteFilePath? file = AbsoluteFilePath.FromExisting(path.Value);
            if (file.Extension == FileExtension.Json)
            {
                using FileStream stream = File.OpenRead(path.Value);
                IReadOnlyList<Segment>? segment =
                    await JsonSerializer.DeserializeAsync(
                        stream,
                        ModelContext.Default.IReadOnlyListSegment);
                if (segment is null)
                {
                    throw new ArgumentException(
                        $"The specified file could not be understood as an image template segment. File: {file.Value}",
                        nameof(settings));
                }

                segments.AddRange(segment);
            }
            else
            {
                files.Add(file);
            }
        }

        return await ImageTemplate.CreateAsync(
            segments,
            files,
            cancellationToken);
    }
}