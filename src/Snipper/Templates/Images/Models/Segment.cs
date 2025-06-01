using System;
using System.Text.Json.Serialization;

namespace Snipper.Templates.Images.Models;

/// <summary>
/// Represents a segment to snip from an image.
/// </summary>
public sealed class Segment
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Segment"/> class.
    /// </summary>
    /// <param name="name">
    /// The unique name of the segment.
    /// </param>
    /// <param name="region">
    /// The segment to snip.
    /// </param>
    /// <param name="pattern">
    /// The pattern to use when snipping, or <see langword="null"/> to not use a pattern.
    /// </param>
    /// <param name="scaling">
    /// The scaling to apply to the output image, or <see langword="null"/> to not apply scaling.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="name"/> or <paramref name="region"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="name"/> is <see cref="string.Empty"/>.
    /// </exception>
    public Segment(
        string name,
        BoundingBox region,
        Pattern? pattern,
        Scaling? scaling)
    {
        Name = name.ThrowIfNull(nameof(name)).ThrowIfEmpty(nameof(name));
        Region = region.ThrowIfNull();
        Scaling = scaling;
    }

    /// <summary>
    /// Gets the name of the segment.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; }

    /// <summary>
    /// Gets the region to snip.
    /// </summary>
    [JsonPropertyName("region")]
    public BoundingBox Region { get; }

    /// <summary>
    /// Gets the pattern to use when snipping.
    /// </summary>
    /// <value>
    /// The pattern to use when snipping, or <see langword="null"/> to not use a pattern.
    /// </value>
    [JsonPropertyName("pattern")]
    public Pattern? Pattern { get; }

    /// <summary>
    /// Gets the scaling to apply to the output image.
    /// </summary>
    /// <value>
    /// The scaling to apply to the output image, or <see langword="null"/> to not apply scaling.
    /// </value>
    [JsonPropertyName("scaling")]
    public Scaling? Scaling { get; }
}