using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Snipper.Tests.Files;

/// <summary>
/// A test fixture for file/directory testing.
/// </summary>
internal sealed class Fixture : IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Fixture"/> class.
    /// </summary>
    /// <param name="type">
    /// The type of absolute path to create.
    /// </param>
    /// <exception cref="AssertInconclusiveException">
    /// Thrown when <paramref name="type"/> is not valid.
    /// </exception>
    public Fixture(PathType type)
    {
        if (!Enum.IsDefined(type))
        {
            throw new AssertInconclusiveException(
                "The specified path type is not recognized as a valid enumeration member.");
        }

        AbsolutePath = Path.GetTempFileName();
        if (type == PathType.Directory)
        {
            File.Delete(AbsolutePath);
            Directory.CreateDirectory(AbsolutePath);
        }
        else if (type == PathType.NotExisting)
        {
            File.Delete(AbsolutePath);
        }

        Type = type;
    }

    /// <summary>
    /// Represents the type of path the fixture contains.
    /// </summary>
    public enum PathType
    {
        /// <summary>
        /// Indicates that the path is a directory.
        /// </summary>
        Directory,

        /// <summary>
        /// Indicates that the path is a file.
        /// </summary>
        File,

        /// <summary>
        /// Indicates that the path represents a file or directory that does not exist.
        /// </summary>
        NotExisting,
    }

    /// <summary>
    /// Gets the absolute path.
    /// </summary>
    public string AbsolutePath { get; }

    public PathType Type { get; }

    /// <inheritdoc/>
    public void Dispose()
    {
        try
        {
            switch (Type)
            {
                case PathType.Directory:
                    Directory.Delete(AbsolutePath);
                    break;
                case PathType.File:
                    File.Delete(AbsolutePath);
                    break;
                case PathType.NotExisting:
                    break;
            }
        }
        catch
        {
            // Just ignore any errors that occur while trying to clean up, assume the test did something funny.
        }
    }
}