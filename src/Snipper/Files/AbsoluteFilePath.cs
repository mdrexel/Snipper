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
        Extension = GetExtensionFrom(path);
        WithoutExtension = GetWithoutExtension(path, Extension?.Normalized);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AbsoluteFilePath"/> class.
    /// </summary>
    [Obsolete("Are you sure you should be going through the protected constructor that leaves properties uninitialized?")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Justification = "We are using it to inject the underlying value from the derived type without repeating validation."
    private AbsoluteFilePath() : base()
    {
    }
#pragma warning restore CS8618

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
    /// Gets the file extension.
    /// </summary>
    /// <value>
    /// The file extension; or, if the file does not have a file extension, <see langword="null"/>.
    /// </value>
    /// <remarks>
    /// Note that a file only has an extension if there's at least one character after the last period. This means a
    /// file name like <c>foo.</c> (with a trailing period and no additional characters) does not have a file extension,
    /// and so this property will be <see langword="null"/>.
    /// </remarks>
    public FileExtension? Extension { get; init; }

    /// <summary>
    /// Gets the string representation of the absolute file path without the trailing file extension.
    /// </summary>
    public string WithoutExtension { get; init; }

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
        FileExtension? extension = GetExtensionFrom(path);
        result =
            new()
            {
                Value = path,
                Extension = extension,
                WithoutExtension = GetWithoutExtension(path, extension?.Normalized),
            };
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

    /// <summary>
    /// Returns the <see cref="FileExtension"/> of the specified <paramref name="path"/>.
    /// </summary>
    /// <param name="path">
    /// The absolute path.
    /// </param>
    /// <returns>
    /// The <see cref="FileExtension"/>, or <see langword="null"/> if <paramref name="path"/> does not have a file
    /// extension.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="path"/> is <see langword="null"/>.
    /// </exception>
    private static FileExtension? GetExtensionFrom(string path)
    {
        path.ThrowIfNull();

        // `GetExtension` only returns `null` when the input value is `null`, which we know is never true.
        string extension = Path.GetExtension(path)!;
        if (extension == string.Empty)
        {
            return null;
        }

        // `GetExtension` includes the leading period, which we don't want.
        string trimmed = extension[1..];

        return new FileExtension(trimmed);
    }

    private static string GetWithoutExtension(string path, string? extension)
    {
        if (extension is null)
        {
            // We can return the path as-is, it already has no extension.
            // (Note that there could be a trailing period. The trailing period counts as part of the file name.)
            return path;
        }

        // Chop off the extension and the period.
        return path[..^(extension.Length + 1)];
    }
}