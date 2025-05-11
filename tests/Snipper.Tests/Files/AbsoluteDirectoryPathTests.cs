using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Snipper.Files;

namespace Snipper.Tests.Files;

[TestClass]
public sealed class AbsoluteDirectoryPathTests
{
    [TestMethod]
    public void CompareTo_AreEqual_ReturnsZero()
    {
        using Fixture fixture = new(Fixture.PathType.Directory);

        AbsoluteDirectoryPath left = AbsoluteDirectoryPath.FromExisting(fixture.AbsolutePath);
        AbsoluteDirectoryPath right = AbsoluteDirectoryPath.FromExisting(fixture.AbsolutePath);

        int actual = left.CompareTo(right);

        Assert.AreEqual(0, actual);
    }

    [TestMethod]
    public void CompareTo_Null_ReturnsPositive()
    {
        using Fixture fixture = new(Fixture.PathType.Directory);

        AbsoluteDirectoryPath left = AbsoluteDirectoryPath.FromExisting(fixture.AbsolutePath);

        int actual = left.CompareTo(null);

        Assert.IsTrue(actual > 0);
    }

    [TestMethod]
    public void CompareTo_AbsolutePath_ReturnsNegative()
    {
        using Fixture fixture = new(Fixture.PathType.Directory);

        AbsoluteDirectoryPath left = AbsoluteDirectoryPath.FromExisting(fixture.AbsolutePath);
        AbsolutePath right = new(fixture.AbsolutePath);

        int actual = left.CompareTo(right);

        Assert.IsTrue(actual < 0);
    }

    [TestMethod]
    public void Equals_AreEqual_ReturnsTrue()
    {
        using Fixture fixture = new(Fixture.PathType.Directory);

        AbsoluteDirectoryPath left = AbsoluteDirectoryPath.FromExisting(fixture.AbsolutePath);
        AbsoluteDirectoryPath right = AbsoluteDirectoryPath.FromExisting(fixture.AbsolutePath);

        bool actual = left.Equals(right);

        Assert.IsTrue(actual);
    }

    [TestMethod]
    public void Equals_AreNotEqual_ReturnsFalse()
    {
        using Fixture fixture1 = new(Fixture.PathType.Directory);
        using Fixture fixture2 = new(Fixture.PathType.Directory);

        AbsoluteDirectoryPath left = AbsoluteDirectoryPath.FromExisting(fixture1.AbsolutePath);
        AbsoluteDirectoryPath right = AbsoluteDirectoryPath.FromExisting(fixture2.AbsolutePath);

        bool actual = left.Equals(right);

        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void Equals_AbsoluteFilePath_ReturnsFalse()
    {
        using Fixture fixture1 = new(Fixture.PathType.Directory);
        using Fixture fixture2 = new(Fixture.PathType.File);

        AbsoluteDirectoryPath left = AbsoluteDirectoryPath.FromExisting(fixture1.AbsolutePath);
        AbsoluteFilePath right = AbsoluteFilePath.FromExisting(fixture2.AbsolutePath);

        bool actual = left.Equals(right);

        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void Equals_AbsolutePath_ReturnsFalse()
    {
        using Fixture fixture = new(Fixture.PathType.Directory);

        AbsoluteDirectoryPath left = AbsoluteDirectoryPath.FromExisting(fixture.AbsolutePath);
        AbsolutePath right = new(fixture.AbsolutePath);

        bool actual = left.Equals(right);

        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void FromExisting_Path_Exists_Succeeds()
    {
        using Fixture fixture = new(Fixture.PathType.Directory);

        AbsoluteDirectoryPath instance = AbsoluteDirectoryPath.FromExisting(fixture.AbsolutePath);

        Assert.AreEqual(fixture.AbsolutePath, instance.Value);
    }

    [TestMethod]
    public void FromExisting_Path_Null_ThrowsArgumentNull()
    {
        Assert.ThrowsExactly<ArgumentNullException>(
            () => AbsoluteDirectoryPath.FromExisting(null!));
    }

    [TestMethod]
    public void FromExisting_Path_DoesNotExist_ThrowsArgument()
    {
        using Fixture fixture = new(Fixture.PathType.NotExisting);

        Assert.ThrowsExactly<ArgumentException>(
            () => AbsoluteDirectoryPath.FromExisting(fixture.AbsolutePath));
    }

    [TestMethod]
    public void FromExisting_Path_ExistingFile_ThrowsArgument()
    {
        using Fixture fixture = new(Fixture.PathType.File);

        Assert.ThrowsExactly<ArgumentException>(
            () => AbsoluteDirectoryPath.FromExisting(fixture.AbsolutePath));
    }
    [TestMethod]
    public void TryFromExisting_Path_Exists_ReturnsTrue()
    {
        using Fixture fixture = new(Fixture.PathType.Directory);

        bool actual = AbsoluteDirectoryPath.TryFromExisting(fixture.AbsolutePath, out AbsoluteDirectoryPath? result);

        Assert.IsTrue(actual);
        Assert.IsNotNull(result);
        Assert.AreEqual(fixture.AbsolutePath, result.Value);
    }

    [TestMethod]
    public void TryFromExisting_Path_Null_ReturnsFalse()
    {
        bool actual = AbsoluteDirectoryPath.TryFromExisting(null!, out AbsoluteDirectoryPath? result);

        Assert.IsFalse(actual);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void TryFromExisting_Path_DoesNotExist_ReturnsFalse()
    {
        using Fixture fixture = new(Fixture.PathType.NotExisting);

        bool actual = AbsoluteDirectoryPath.TryFromExisting(fixture.AbsolutePath, out AbsoluteDirectoryPath? result);

        Assert.IsFalse(actual);
        Assert.IsNull(result);
    }

    [TestMethod]
    public void TryFromExisting_Path_ExistingFile_ReturnsFalse()
    {
        using Fixture fixture = new(Fixture.PathType.File);

        bool actual = AbsoluteDirectoryPath.TryFromExisting(fixture.AbsolutePath, out AbsoluteDirectoryPath? result);

        Assert.IsFalse(actual);
        Assert.IsNull(result);
    }
}