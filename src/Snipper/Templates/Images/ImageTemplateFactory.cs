using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        List<AbsoluteFilePath> paths = [];
        foreach (AbsolutePath path in settings.Paths)
        {
            if (!AbsoluteFilePath.TryFromExisting(path.Value, out AbsoluteFilePath? file))
            {
                // All specified paths must be existing files.
                template = default;
                return false;
            }

            ////if (StringComparer.OrdinalIgnoreCase.Equals(file.Extension, ".JSON"))
            ////{

            ////}
            ////else if (ImageTemplate.FileTypes.Contains(file.Extension))
            ////{

            ////}
        }

        template = default;
        return false;
    }
}