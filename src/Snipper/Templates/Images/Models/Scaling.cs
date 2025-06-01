using System.Text.Json.Serialization;

namespace Snipper.Templates.Images.Models;

/// <summary>
/// The information associated with scaling an image.
/// </summary>
public sealed class Scaling
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Scaling"/> class.
    /// </summary>
    /// <param name="factor">
    /// The integer scaling factor.
    /// </param>
    /// <param name="mode">
    /// The scaling mode, or <see langword="null"/> to use the default.
    /// </param>
    public Scaling(
        uint factor,
        InterpolationMode? mode)
    {
        Factor = factor;
        Mode = mode;
    }

    /// <summary>
    /// Gets the integer scaling factor.
    /// </summary>
    [JsonPropertyName("factor")]
    public uint Factor { get; }

    /// <summary>
    /// Gets the scaling mode.
    /// </summary>
    /// <value>
    /// The scaling mode, or <see langword="null"/> to use the default value.
    /// </value>
    [JsonPropertyName("mode")]
    [JsonConverter(typeof(JsonStringEnumConverter<InterpolationMode>))]
    public InterpolationMode? Mode { get; }
}