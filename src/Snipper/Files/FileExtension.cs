using System;

namespace Snipper.Files;

/// <summary>
/// Represents a file extension.
/// </summary>
public sealed class FileExtension : IEquatable<FileExtension>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileExtension"/> class.
    /// </summary>
    /// <param name="extension">
    /// The file extension, not including the leading period.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="extension"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="extension"/> is <see cref="string.Empty"/> or contains a period.
    /// </exception>
    public FileExtension(string extension)
    {
        extension.ThrowIfNull();
        extension.ThrowIfEmpty();
        if (extension.Contains('.'))
        {
            throw new ArgumentException(
                "File extension string representation cannot contain a period.",
                nameof(extension));
        }

        Normalized = extension.ToLowerInvariant();
    }

    /// <summary>
    /// Gets the <c>"bmp"</c> file extension.
    /// </summary>
    public static FileExtension Bmp { get; } = new("bmp");

    /// <summary>
    /// Gets the <c>"jpeg"</c> file extension.
    /// </summary>
    public static FileExtension Jpeg { get; } = new("jpeg");

    /// <summary>
    /// Gets the <c>"jpg"</c> file extension.
    /// </summary>
    public static FileExtension Jpg { get; } = new("jpg");

    /// <summary>
    /// Gets the <c>"json"</c> file extension.
    /// </summary>
    public static FileExtension Json { get; } = new("json");

    /// <summary>
    /// Gets the <c>"png"</c> file extension.
    /// </summary>
    public static FileExtension Png { get; } = new("png");

    /// <summary>
    /// Gets the normalized string representation of the file extension.
    /// </summary>
    public string Normalized { get; }

    /// <inheritdoc/>
    public static bool operator ==(FileExtension? left, FileExtension? right) =>
        left is null ? right is null : left.Equals(right);

    /// <inheritdoc/>
    public static bool operator !=(FileExtension? left, FileExtension? right) =>
        !(left == right);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => this.Equals(obj as FileExtension);

    /// <inheritdoc/>
    public bool Equals(FileExtension? other)
    {
        if (other is null)
        {
            return false;
        }

        return StringComparer.Ordinal.Equals(this.Normalized, other.Normalized);
    }

    /// <inheritdoc/>
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Normalized);

    /// <inheritdoc/>
    public override string ToString() => Normalized;
}