namespace Snipper.Templates.Images.Models;

/// <summary>
/// The type of interpolation mode to use when scaling an image.
/// </summary>
public enum InterpolationMode
{
    /// <summary>
    /// Indicates the image should be scaled using nearest neighbour interpolation.
    /// </summary>
    NearestNeighbour = 0,

    /// <summary>
    /// Indicates the image should be scaled using bilinear interpolation.
    /// </summary>
    Bilinear = 1,

    /// <summary>
    /// Indicates the image should be scaled using bicubic interpolation.
    /// </summary>
    Bicubic = 2,
}