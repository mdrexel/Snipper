using System;
using System.Collections.Generic;

namespace Snipper;

/// <summary>
/// Contains application settings shared across all template implementations.
/// </summary>
public sealed class TemplateSettings
{
    public IReadOnlyList<string> Input { get; }
}