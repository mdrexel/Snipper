using System;
using System.Diagnostics.CodeAnalysis;
using Snipper.Files;

namespace Snipper.Templates;

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

        foreach (AbsolutePath path in settings.Paths)
        {

        }

        template = default;
        return false;
    }
}