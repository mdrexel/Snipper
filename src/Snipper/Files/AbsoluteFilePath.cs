using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Snipper.Files;

/// <summary>
/// Represents the absolute path of a file.
/// </summary>
/// <remarks>
/// Note that the file might not exist anymore if it has been deleted, but it existed when the instance was created.
/// </remarks>
public sealed class AbsoluteFilePath : AbsolutePath
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AbsoluteFilePath"/> class.
    /// </summary>
    /// <param name="path">
    /// The absolute file path.
    /// </param>
    /// <inheritdoc/>
    private AbsoluteFilePath(string path) : base(path)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AbsoluteFilePath"/> class.
    /// </summary>
    [Obsolete("Are you sure you should be going through the protected constructor that leaves properties uninitialized?")]
    private AbsoluteFilePath() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AbsoluteFilePath"/> class from the specified existing
    /// <paramref name="path"/> that the caller has permissions to read.
    /// </summary>
    /// <param name="path">
    /// The absolute file path of an existing file that the caller has permissions to read.
    /// </param>
    /// <returns>
    /// A new instance of the <see cref="AbsoluteFilePath"/> class derived from the specified existing
    /// <paramref name="path"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="path"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="path"/> is not an absolute file path for an existing file that can be read.
    /// </exception>
    public static AbsoluteFilePath FromExisting(string path)
    {
        AbsoluteFilePath instance = new(path);

        if (!File.Exists(instance.Value))
        {
            throw new ArgumentException(
                $"The specified path does not represent an existing file that can be read. Path: {path}",
                nameof(path));
        }

        return instance;
    }

    /// <summary>
    /// Tries to initialize a new instance of the <see cref="AbsoluteFilePath"/> class from the specified existing
    /// <paramref name="path"/> that the caller has permissions to read.
    /// </summary>
    /// <param name="path">
    /// The absolute file path of an existing file that the caller has permissions to read.
    /// </param>
    /// <param name="result">
    /// When this method returns <see langword="true"/>, set to a new instance of the <see cref="AbsoluteFilePath"/>
    /// class derived from the specified existing <paramref name="path"/>; otherwise, set to <see langword="null"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="result"/> was set to a new <see cref="AbsoluteFilePath"/> instance
    /// initialized using the specified <paramref name="path"/>; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool TryFromExisting(
        string path,
        [NotNullWhen(returnValue: true)] out AbsoluteFilePath? result)
    {
        if (!AbsolutePath.TryParse(path, out _))
        {
            result = default;
            return false;
        }
        else if (!File.Exists(path))
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
    public override bool Equals(AbsolutePath? other)
    {
        if (other is not AbsoluteFilePath)
        {
            return false;
        }

        return base.Equals(other);
    }

    /// <inheritdoc/>
    public override int CompareTo(AbsolutePath? other)
    {
        if (other is null)
        {
            // Null always appears first.
            return 1;
        }
        else if (other is AbsoluteFilePath)
        {
            return base.CompareTo(other);
        }
        else
        {
            // Files always appear last.
            return 1;
        }
    }
}