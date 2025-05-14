namespace Snipper.Templates.Images;

/// <summary>
/// Represents a segment to snip from an image.
/// </summary>
public sealed class Segment
{
    /// <summary>
    /// Gets the name of the segment.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the X coordinate to start snipping from.
    /// </summary>
    public required uint X { get; init; }

    /// <summary>
    /// Gets the Y coordinate to start snipping from.
    /// </summary>
    public required uint Y { get; init; }

    /// <summary>
    /// Gets the number of pixels to snip tall-ways.
    /// </summary>
    public required uint Height { get; init; }

    /// <summary>
    /// Gets the number of pixels to snip wide-ways.
    /// </summary>
    public required uint Width { get; init; }
}