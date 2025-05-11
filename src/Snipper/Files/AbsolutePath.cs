using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Snipper.Files;

/// <summary>
/// Represents an absolute path.
/// </summary>
public class AbsolutePath : IEquatable<AbsolutePath>, IComparable<AbsolutePath>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AbsolutePath"/> class.
    /// </summary>
    /// <param name="path">
    /// The absolute path.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="path"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="path"/> does not represent an absolute path.
    /// </exception>
    public AbsolutePath(string path)
    {
        path.ThrowIfNull();
        if (!Path.IsPathFullyQualified(path))
        {
            throw new ArgumentException(
                "The specified path is not fully qualified.",
                nameof(path));
        }

        Value = path;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AbsolutePath"/> class.
    /// </summary>
    /// <remarks>
    /// The derived type is responsible for populating the <see cref="Value"/> field.
    /// </remarks>
    [Obsolete("Are you sure you should be going through the protected constructor that leaves properties uninitialized?")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Justification = "We are using it to inject the underlying value from the derived type without repeating validation."
    protected AbsolutePath()
#pragma warning restore CS8618
    {
    }

    /// <summary>
    /// Gets the string representation of the absolute path.
    /// </summary>
    public string Value { get; protected init; }

    /// <inheritdoc/>
    public static bool operator ==(AbsolutePath? left, AbsolutePath? right) =>
        left is null ? right is null : left.Equals(right);

    /// <inheritdoc/>
    public static bool operator !=(AbsolutePath? left, AbsolutePath? right) =>
        !(left == right);

    /// <inheritdoc/>
    public static bool operator <(AbsolutePath? left, AbsolutePath? right) =>
        left is null ? right is not null : left.CompareTo(right) < 0;

    /// <inheritdoc/>
    public static bool operator >(AbsolutePath? left, AbsolutePath? right) =>
        left is null ? false : left.CompareTo(right) > 0;

    /// <inheritdoc/>
    public static bool operator <=(AbsolutePath? left, AbsolutePath? right) =>
        left is null ? true : left.CompareTo(right) <= 0;

    /// <inheritdoc/>
    public static bool operator >=(AbsolutePath? left, AbsolutePath? right) =>
        left is null ? right is null : left.CompareTo(right) >= 0;

    /// <summary>
    /// Tries to initialize a new <see cref="AbsolutePath"/> instance using the specified <paramref name="path"/>.
    /// </summary>
    /// <param name="path">
    /// The absolute path.
    /// </param>
    /// <param name="result">
    /// When this method returns <see langword="true"/>, set to the <see cref="AbsolutePath"/> instance initialized
    /// using the specified <paramref name="path"/>; otherwise, set to <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="result"/> was set to a new <see cref="AbsolutePath"/> instance
    /// initialized using the specified <paramref name="path"/>; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool TryParse(
        string path,
        [NotNullWhen(returnValue: true)] out AbsolutePath? result)
    {
        if (path is null)
        {
            result = default;
            return false;
        }
        else if (!Path.IsPathFullyQualified(path))
        {
            result = default;
            return false;
        }

#pragma warning disable CS0618 // Type or member is obsolete, Justification = "It's not actually obsolete."
        result = new() { Value = path };
#pragma warning restore CS0618

        return true;
    }

    /// <inheritdoc/>
    public virtual int CompareTo(AbsolutePath? other) => StringComparer.Ordinal.Compare(this.Value, other?.Value);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => this.Equals(obj as AbsolutePath);

    /// <inheritdoc/>
    public virtual bool Equals(AbsolutePath? other)
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