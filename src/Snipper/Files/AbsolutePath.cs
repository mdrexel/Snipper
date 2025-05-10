using System;
using System.IO;

namespace Snipper.Files;

/// <summary>
/// Represents an absolute path.
/// </summary>
internal abstract class AbsolutePath : IEquatable<AbsolutePath>, IComparable<AbsolutePath>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AbsolutePath"/> class.
    /// </summary>
    /// <param name="path"></param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="path"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="path"/> does not represent an absolute path.
    /// </exception>
    protected AbsolutePath(string path)
    {
        ArgumentNullException.ThrowIfNull(path);
        if (!Path.IsPathFullyQualified(path))
        {
            throw new ArgumentException(
                "The specified path is not fully qualified.",
                nameof(path));
        }

        Value = path;
    }

    /// <summary>
    /// Gets the string representation of the absolute path.
    /// </summary>
    public string Value { get; }

    /// <inheritdoc/>
    public static bool operator ==(AbsolutePath? left, AbsolutePath? right) =>
        left is null ? right is null : left.Equals(right);

    /// <inheritdoc/>
    public static bool operator !=(AbsolutePath? left, AbsolutePath? right) =>
        !(left == right);

    /// <inheritdoc/>
    public static bool operator <(AbsolutePath? left, AbsolutePath? right) =>
        left is null ? right is null : left.CompareTo(right) < 0;

    /// <inheritdoc/>
    public static bool operator >(AbsolutePath? left, AbsolutePath? right) =>
        left is null ? right is not null : left.CompareTo(right) > 0;

    /// <inheritdoc/>
    public static bool operator <=(AbsolutePath? left, AbsolutePath? right) =>
        left is null ? right is null : left.CompareTo(right) <= 0;

    /// <inheritdoc/>
    public static bool operator >=(AbsolutePath? left, AbsolutePath? right) =>
        left is null ? right is null : left.CompareTo(right) >= 0;

    /// <inheritdoc/>
    public int CompareTo(AbsolutePath? other) => StringComparer.Ordinal.Compare(this.Value, other?.Value);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => this.Equals(obj as AbsolutePath);

    /// <inheritdoc/>
    public bool Equals(AbsolutePath? other)
    {
        if (other is null)
        {
            return false;
        }

        return StringComparer.Ordinal.Equals(Value, other.Value);
    }

    /// <inheritdoc/>
    public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Value);

    /// <inheritdoc/>
    public override string ToString() => Value;
}