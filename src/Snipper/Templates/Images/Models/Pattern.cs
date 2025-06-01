using System.Text.Json.Serialization;

namespace Snipper.Templates.Images.Models;

/// <summary>
/// Describes a pattern to use when snipping.
/// </summary>
public sealed class Pattern
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Pattern"/> class.
    /// </summary>
    /// <param name="cellSize">
    /// The cell size to use, or <see langword="null"/> to use the segment's bounding box as the cell size.
    /// </param>
    /// <param name="horizontalCount">
    /// The number of horizontal cells, or <see langword="null"/> if there is no horizontal cell limit.
    /// </param>
    /// <param name="verticalCount">
    /// The number of vertical cells, or <see langword="null"/> if there is no vertical cell limit.
    /// </param>
    public Pattern(
        BoundingBox? cellSize,
        ushort? horizontalCount,
        ushort? verticalCount)
    {
        CellSize = cellSize;
        HorizontalCount = horizontalCount;
        VerticalCount = verticalCount;
    }

    /// <summary>
    /// Gets the cell size to use for the pattern. 
    /// </summary>
    /// <value>
    /// The cell size to use for the pattern, or <see langword="null"/> to use the segment's bounding box as the cell
    /// size.
    /// </value>
    [JsonPropertyName("cellSize")]
    public BoundingBox? CellSize { get; }

    /// <summary>
    /// Gets the number of horizontal cells this pattern includes.
    /// </summary>
    /// <value>
    /// The number of horizontal cells, or <see langword="null"/> if there is no horizontal cell limit.
    /// </value>
    [JsonPropertyName("horizontalCount")]
    public ushort? HorizontalCount { get; }

    /// <summary>
    /// Gets the number of vertical cells this pattern includes.
    /// </summary>
    /// <value>
    /// The number of vertical cells, or <see langword="null"/> if there is no vertical cell limit.
    /// </value>
    [JsonPropertyName("verticalCount")]
    public ushort? VerticalCount { get; }
}