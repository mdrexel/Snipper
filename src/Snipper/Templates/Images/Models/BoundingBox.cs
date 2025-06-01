using System.Text.Json.Serialization;

namespace Snipper.Templates.Images.Models;

/// <summary>
/// Represents a bounding box.
/// </summary>
public sealed class BoundingBox
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BoundingBox"/> class.
    /// </summary>
    /// <param name="x">
    /// The X coordinate of the top-left of the box.
    /// </param>
    /// <param name="y">
    /// The Y coordinate of the top-left of the box.
    /// </param>
    /// <param name="height">
    /// The height of the box.
    /// </param>
    /// <param name="width">
    /// The width of the box.
    /// </param>
    public BoundingBox(
        ushort x,
        ushort y,
        ushort height,
        ushort width)
    {
        X = x;
        Y = y;
        Height = height;
        Width = width;
    }

    /// <summary>
    /// Gets the X coordinate of the top-left of the box.
    /// </summary>
    [JsonPropertyName("x")]
    public ushort X { get; }

    /// <summary>
    /// Gets the Y coordinate of the top-left of the box.
    /// </summary>
    [JsonPropertyName("y")]
    public ushort Y { get; }

    /// <summary>
    /// Gets the height of the box.
    /// </summary>
    [JsonPropertyName("height")]
    public ushort Height { get; }

    /// <summary>
    /// Gets the width of the box.
    /// </summary>
    [JsonPropertyName("width")]
    public ushort Width { get; }
}