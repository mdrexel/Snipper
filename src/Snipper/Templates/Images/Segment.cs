using System;
using System.Text.Json.Serialization;

namespace Snipper.Templates.Images;

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
    /// <param name="x">
    /// The X coordinate to start snipping from.
    /// </param>
    /// <param name="y">
    /// The Y coordinate to start snipping from.
    /// </param>
    /// <param name="height">
    /// The number of pixels to snip tall-ways.
    /// </param>
    /// <param name="width">
    /// The number of pixels to snip wide-ways.
    /// </param>
    /// <param name="scale">
    /// The scaling factor.
    /// </param>
    /// <param name="scaleMode">
    /// The scaling mode, or <see langword="null"/> to use the default value.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="name"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="name"/> is <see cref="string.Empty"/>.
    /// </exception>
    public Segment(
        string name,
        ushort x,
        ushort y,
        ushort height,
        ushort width,
        ushort scale,
        InterpolationMode? scaleMode)
    {
        Name = name.ThrowIfNull(nameof(name)).ThrowIfEmpty(nameof(name));
        X = x;
        Y = y;
        Height = height;
        Width = width;
        Scale = scale;
        ScaleMode = scaleMode;
    }

    /// <summary>
    /// Gets the name of the segment.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; }

    /// <summary>
    /// Gets the X coordinate to start snipping from.
    /// </summary>
    [JsonPropertyName("x")]
    public ushort X { get; }

    /// <summary>
    /// Gets the Y coordinate to start snipping from.
    /// </summary>
    [JsonPropertyName("y")]
    public ushort Y { get; }

    /// <summary>
    /// Gets the number of pixels to snip tall-ways.
    /// </summary>
    [JsonPropertyName("height")]
    public ushort Height { get; }

    /// <summary>
    /// Gets the number of pixels to snip wide-ways.
    /// </summary>
    [JsonPropertyName("width")]
    public ushort Width { get; }

    /// <summary>
    /// Gets the scaling factor to use when snipping.
    /// </summary>
    [JsonPropertyName("scale")]
    public ushort Scale { get; }

    /// <summary>
    /// Gets the scaling mode to use when snipping.
    /// </summary>
    /// <value>
    /// The scaling mode to use when snipping, or <see langword="null"/> to use the default value.
    /// </value>
    [JsonPropertyName("scaleMode")]
    [JsonConverter(typeof(JsonStringEnumConverter<InterpolationMode>))]
    public InterpolationMode? ScaleMode { get; }
}