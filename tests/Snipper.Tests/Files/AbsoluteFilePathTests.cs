﻿using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Snipper.Files;

namespace Snipper.Tests.Files;

[TestClass]
public sealed class AbsoluteFilePathTests
{
    public static IEnumerable<object?[]> ExtensionCases { get; } =
        new object?[][]
        {
            [@"foo", null],
            [@"foo.", null],
            [@"foo.txt", new FileExtension("txt")],
            [@"foo.gif", new FileExtension("gif")],
            [@"foo.bar.baz", new FileExtension("baz")]
        };

    public static IEnumerable<object[]> WithoutExtensionCases { get; } =
        new object[][]
        {
            [@"foo", @"foo"],
            [@"foo.", @"foo."],
            [@"foo.txt", @"foo"],
            [@"foo.gif", @"foo"],
            [@"foo.bar.baz", @"foo.bar"],
        };

    [TestMethod]
    public void CompareTo_AreEqual_ReturnsZero()
    {
        using Fixture fixture = new(Fixture.PathType.File);

        AbsoluteFilePath left = AbsoluteFilePath.FromExisting(fixture.AbsolutePath);
        AbsoluteFilePath right = AbsoluteFilePath.FromExisting(fixture.AbsolutePath);

        int actual = left.CompareTo(right);

        Assert.AreEqual(0, actual);
    }

    [TestMethod]
    public void CompareTo_Null_ReturnsPositive()
    {
        using Fixture fixture = new(Fixture.PathType.File);

        AbsoluteFilePath left = AbsoluteFilePath.FromExisting(fixture.AbsolutePath);

        int actual = left.CompareTo(null);

        Assert.IsTrue(actual > 0);
    }

    [TestMethod]
    public void CompareTo_AbsolutePath_ReturnsPositive()
    {
        using Fixture fixture = new(Fixture.PathType.File);

        AbsoluteFilePath left = AbsoluteFilePath.FromExisting(fixture.AbsolutePath);
        AbsolutePath right = new(fixture.AbsolutePath);

        int actual = left.CompareTo(right);

        Assert.IsTrue(actual > 0);
    }

    [TestMethod]
    public void Equals_AreEqual_ReturnsTrue()
    {
        using Fixture fixture = new(Fixture.PathType.File);

        AbsoluteFilePath left = AbsoluteFilePath.FromExisting(fixture.AbsolutePath);
        AbsoluteFilePath right = AbsoluteFilePath.FromExisting(fixture.AbsolutePath);

        bool actual = left.Equals(right);

        Assert.IsTrue(actual);
    }

    [TestMethod]
    public void Equals_AreNotEqual_ReturnsFalse()
    {
        using Fixture fixture1 = new(Fixture.PathType.File);
        using Fixture fixture2 = new(Fixture.PathType.File);

        AbsoluteFilePath left = AbsoluteFilePath.FromExisting(fixture1.AbsolutePath);
        AbsoluteFilePath right = AbsoluteFilePath.FromExisting(fixture2.AbsolutePath);

        bool actual = left.Equals(right);

        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void Equals_AbsoluteDirectoryPath_ReturnsFalse()
    {
        using Fixture fixture1 = new(Fixture.PathType.File);
        using Fixture fixture2 = new(Fixture.PathType.Directory);

        AbsoluteFilePath left = AbsoluteFilePath.FromExisting(fixture1.AbsolutePath);
        AbsoluteDirectoryPath right = AbsoluteDirectoryPath.FromExisting(fixture2.AbsolutePath);

        bool actual = left.Equals(right);

        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void Equals_AbsolutePath_ReturnsFalse()
    {
        using Fixture fixture = new(Fixture.PathType.File);

        AbsoluteFilePath left = AbsoluteFilePath.FromExisting(fixture.AbsolutePath);
        AbsolutePath right = new(fixture.AbsolutePath);

        bool actual = left.Equals(right);

        Assert.IsFalse(actual);
    }

    [TestMethod]
    [DynamicData(nameof(ExtensionCases))]
    public void Extension_Succeeds(string fileName, FileExtension? expected)
    {
        using Fixture fixture = new Fixture(Fixture.PathType.Directory);
        string path = Path.Combine(fixture.AbsolutePath, fileName);
        File.Create(path).Dispose();

        AbsoluteFilePath instance = AbsoluteFilePath.FromExisting(path);

        Assert.AreEqual(expected, instance.Extension);
    }

    [TestMethod]
    public void FromExisting_Path_Exists_Succeeds()
    {
        using Fixture fixture = new(Fixture.PathType.File);

        AbsoluteFilePath instance = AbsoluteFilePath.FromExisting(fixture.AbsolutePath);

        Assert.AreEqual(fixture.AbsolutePath, instance.Value);
    }

    [TestMethod]
    public void FromExisting_Path_Null_ThrowsArgumentNull()
    {
        Assert.ThrowsExactly<ArgumentNullException>(
            () => AbsoluteFilePath.FromExisting(null!));
    }

    [TestMethod]
    public void FromExisting_Path_DoesNotExist_ThrowsArgument()
    {
        using Fixture fixture = new(Fixture.PathType.NotExisting);

        Assert.ThrowsExactly<ArgumentException>(
            () => AbsoluteFilePath.FromExisting(fixture.AbsolutePath));
    }

    [TestMethod]
    public void FromExisting_Path_ExistingDirectory_ThrowsArgument()
    {
        using Fixture fixture = new(Fixture.PathType.Directory);

        Assert.ThrowsExactly<ArgumentException>(
            () => AbsoluteFilePath.FromExisting(fixture.AbsolutePath));
    }
    [TestMethod]
    public void TryFromExisting_Path_Exists_ReturnsTrue()
    {
        using Fixture fixture = new(Fixture.PathType.File);

        bool actual = AbsoluteFilePath.TryFromExisting(fixture.AbsolutePath, out AbsoluteFilePath? result);

        Assert.IsTrue(actual);
        Assert.IsNotNull(result);
        Assert.AreEqual(fixture.AbsolutePath, result.Value);
    }

    [TestMethod]
    public void TryFromExisting_Path_Null_ReturnsFalse()
    {
        bool actual = AbsoluteFilePath.TryFromExisting(null!, out AbsoluteFilePath? result);

        Assert.IsFalse(actual);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void TryFromExisting_Path_DoesNotExist_ReturnsFalse()
    {
        using Fixture fixture = new(Fixture.PathType.NotExisting);

        bool actual = AbsoluteFilePath.TryFromExisting(fixture.AbsolutePath, out AbsoluteFilePath? result);

        Assert.IsFalse(actual);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void TryFromExisting_Path_ExistingDirectory_ReturnsFalse()
    {
        using Fixture fixture = new(Fixture.PathType.Directory);

        bool actual = AbsoluteFilePath.TryFromExisting(fixture.AbsolutePath, out AbsoluteFilePath? result);

        Assert.IsFalse(actual);
        Assert.IsNull(result);
    }

    [TestMethod]
    [DynamicData(nameof(WithoutExtensionCases))]
    public void WithoutExtension(string withExtension, string withoutExtension)
    {
        using Fixture fixture = new Fixture(Fixture.PathType.Directory);
        string path = Path.Combine(fixture.AbsolutePath, withExtension);
        string expected = Path.Combine(fixture.AbsolutePath, withoutExtension);
        File.Create(path).Dispose();

        AbsoluteFilePath instance = AbsoluteFilePath.FromExisting(path);

        Assert.AreEqual(expected, instance.WithoutExtension);
    }
}