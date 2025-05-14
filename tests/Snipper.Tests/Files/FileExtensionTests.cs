using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Snipper.Files;

namespace Snipper.Tests.Files;

[TestClass]
public sealed class FileExtensionTests
{
    public static IEnumerable<object[]> AreEqualCases { get; } =
        new object[][]
        {
            [new FileExtension("foo"), new FileExtension("foo")],
            [new FileExtension("foo"), new FileExtension("FOO")],
            [new FileExtension("bar"), new FileExtension("bar")],
        };

    public static IEnumerable<object?[]> AreNotEqualCases { get; } =
        new object?[][]
        {
            [null, new FileExtension("foo")],
            [new FileExtension("foo"), null],
            [new FileExtension("foo"), new FileExtension("bar")],
        };

    public static IEnumerable<object?[]> AreNotEqualObjectCases { get; } =
        new object?[][]
        {
            [new FileExtension("foo"), "foo"],
        };

    [TestMethod]
    [DataRow("foo", "foo")]
    [DataRow("FOO", "foo")]
    [DataRow("bar", "bar")]
    public void Ctor_Succeeds(string input, string expected)
    {
        FileExtension instance = new(input);

        Assert.AreEqual(expected, instance.Normalized);
    }

    [TestMethod]
    public void Ctor_Null_ThrowsArgumentNull()
    {
        ArgumentNullException actual = Assert.ThrowsExactly<ArgumentNullException>(
            () => new FileExtension(null!));

        Assert.AreEqual("extension", actual.ParamName);
    }

    [TestMethod]
    public void Ctor_Empty_ThrowsArgument()
    {
        ArgumentException actual = Assert.ThrowsExactly<ArgumentException>(
            () => new FileExtension(string.Empty));

        Assert.AreEqual("extension", actual.ParamName);
    }

    [TestMethod]
    [DataRow(".")]
    [DataRow(".foo")]
    [DataRow("foo.")]
    [DataRow(".foo.")]
    [DataRow(".foo.bar")]
    [DataRow("foo.bar")]
    [DataRow("foo.bar.")]
    [DataRow(".foo.bar.")]
    public void Ctor_ContainsPeriod_ThrowsArgument(string input)
    {
        ArgumentException actual = Assert.ThrowsExactly<ArgumentException>(
            () => new FileExtension(input));

        Assert.AreEqual("extension", actual.ParamName);
    }

    [TestMethod]
    [DynamicData(nameof(AreEqualCases))]
    public void Equals_AreEqual_ReturnsTrue(FileExtension left, FileExtension right)
    {
        bool actual = left.Equals(right);

        Assert.IsTrue(actual);
    }

    [TestMethod]
    [DynamicData(nameof(AreNotEqualCases))]
    public void Equals_AreNotEqual_ReturnsFalse(FileExtension? left, FileExtension? right)
    {
        if (left is null)
        {
            // Can't invoke instance method on `null`.
            return;
        }

        bool actual = left.Equals(right);

        Assert.IsFalse(actual);
    }

    [TestMethod]
    [DynamicData(nameof(AreEqualCases))]
    public void Equals_Object_AreEqual_ReturnsTrue(FileExtension left, object right)
    {
        bool actual = left.Equals(right);

        Assert.IsTrue(actual);
    }

    [TestMethod]
    [DynamicData(nameof(AreNotEqualCases))]
    [DynamicData(nameof(AreNotEqualObjectCases))]
    public void Equals_Object_AreNotEqual_ReturnsFalse(FileExtension? left, object? right)
    {
        if (left is null)
        {
            // Can't invoke instance method on `null`.
            return;
        }

        bool actual = left.Equals(right);

        Assert.IsFalse(actual);
    }

    [TestMethod]
    [DynamicData(nameof(AreEqualCases))]
    public void GetHashCode_Succeeds(FileExtension left, FileExtension right)
    {
        int leftHash = left.GetHashCode();
        int rightHash = right.GetHashCode();

        Assert.AreEqual(leftHash, rightHash);
    }

    [TestMethod]
    [DynamicData(nameof(AreEqualCases))]
    [DataRow(null, null)]
    public void Operator_Equals_AreEqual_ReturnsTrue(FileExtension? left, FileExtension? right)
    {
        bool actual = left == right;

        Assert.IsTrue(actual);
    }

    [TestMethod]
    [DynamicData(nameof(AreNotEqualCases))]
    public void Operator_Equals_AreNotEqual_ReturnsFalse(FileExtension? left, FileExtension? right)
    {
        bool actual = left == right;

        Assert.IsFalse(actual);
    }

    [TestMethod]
    [DynamicData(nameof(AreEqualCases))]
    [DataRow(null, null)]
    public void Operator_NotEquals_AreEqual_ReturnsFalse(FileExtension? left, FileExtension? right)
    {
        bool actual = left != right;

        Assert.IsFalse(actual);
    }

    [TestMethod]
    [DynamicData(nameof(AreNotEqualCases))]
    public void Operator_NotEquals_AreNotEqual_ReturnsTrue(FileExtension? left, FileExtension? right)
    {
        bool actual = left != right;

        Assert.IsTrue(actual);
    }

    [TestMethod]
    [DataRow("foo", "foo")]
    [DataRow("FOO", "foo")]
    [DataRow("bar", "bar")]
    public void ToString_Succeeds(string input, string expected)
    {
        FileExtension instance = new(input);

        string actual = instance.ToString();

        Assert.AreEqual(expected, actual);
    }
}