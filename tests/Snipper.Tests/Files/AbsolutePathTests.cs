using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Snipper.Files;

namespace Snipper.Tests.Files;

[TestClass]
internal sealed class AbsolutePathTests
{
    public static IEnumerable<object[]> AbsolutePaths { get; } =
        new object[][]
        {
            [@"C:\"],
            [@"C:\foo"],
            [@"C:\foo\bar.txt"],
            [@"C:\foo\bar\"],
            [@"C:\foo\bar\baz.txt"],
            [@"X:\"],
            [@"X:\qux\corge"],
            [@"X:\qux\corge\grault.txt"],
        };

    public static IEnumerable<object[]> AreEqualCases { get; } =
        AbsolutePaths
            .Select(
                x =>
                new object[]
                {
                    new AbsolutePath((string)x[0]),
                    new AbsolutePath((string)x[0]),
                });

    public static IEnumerable<object?[]> AreNotEqualCases
    {
        get
        {
            yield return [null, new AbsolutePath(@"C:\foo\bar.txt")];
            yield return [new AbsolutePath(@"C:\foo\bar.txt"), null];

            using IEnumerator<object[]> enumerator = AbsolutePaths.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                yield break;
            }

            string first = (string)enumerator.Current[0];
            string previous = first;
            if (!enumerator.MoveNext())
            {
                yield break;
            }

            do
            {
                yield return [new AbsolutePath(previous), new AbsolutePath(previous.ToLowerInvariant())];

                string current = (string)enumerator.Current[0];
                yield return [new AbsolutePath(previous), new AbsolutePath(current)];
                previous = current;
            }
            while (enumerator.MoveNext());

            yield return [new AbsolutePath(previous), new AbsolutePath(first)];
        }
    }

    public static IEnumerable<object?[]> LessThanCases =>
        new object?[][]
        {
            [null, new AbsolutePath(@"X:\")],
            [new AbsolutePath(@"A:\foo\bar"), new AbsolutePath(@"B:\foo\bar")],
            [new AbsolutePath(@"A:\foo\bar"), new AbsolutePath(@"a:\foo\bar")],
        };

    public static IEnumerable<object?[]> GreaterThanCases =>
        LessThanCases.Select(x => new object?[] { x[1], x[0] });

    [TestMethod]
    [DynamicData(nameof(AbsolutePaths))]
    public void Ctor_Path_IsAbsolute_Succeeds(string input)
    {
        AbsolutePath instance = new(input);

        Assert.AreEqual(instance.Value, input);
    }

    [TestMethod]
    public void Ctor_Path_Null_ThrowsArgumentNull()
    {
        ArgumentNullException actual = Assert.ThrowsExactly<ArgumentNullException>(
            () => new AbsolutePath(null!));

        Assert.AreEqual("path", actual.ParamName);
    }

    [TestMethod]
    public void Ctor_Path_Empty_ThrowsArgument()
    {
        ArgumentException actual = Assert.ThrowsExactly<ArgumentException>(
            () => new AbsolutePath(string.Empty));

        Assert.AreEqual("path", actual.ParamName);
    }

    [TestMethod]
    [DataRow(@"foo")]
    [DataRow(@"foo.txt")]
    [DataRow(@"foo\bar")]
    [DataRow(@"foo\bar.txt")]
    [DataRow(@"foo\bar\baz")]
    [DataRow(@"/foo")]
    [DataRow(@"/foo/bar.txt")]
    [DataRow(@"./foo/bar.txt")]
    [DataRow(@"../foo/bar.txt")]
    [DataRow(@"foo/../bar/baz")]
    [DataRow(@"foo/../bar/baz.txt")]
    public void Ctor_Path_IsNotAbsolute_ThrowsArgument(string input)
    {
        ArgumentException actual = Assert.ThrowsExactly<ArgumentException>(
            () => new AbsolutePath(input));

        Assert.AreEqual("path", actual.ParamName);
    }

    [TestMethod]
    [DynamicData(nameof(AreEqualCases))]
    public void Equals_AreEqual_ReturnsTrue(AbsolutePath left, AbsolutePath right)
    {
        bool actual = left.Equals(right);

        Assert.IsTrue(actual);
    }

    [TestMethod]
    [DynamicData(nameof(AreNotEqualCases))]
    public void Equals_AreNotEqual_ReturnsFalse(AbsolutePath? left, AbsolutePath? right)
    {
        if (left is null)
        {
            // Can't invoke the instance method on `null`.
            return;
        }

        bool actual = left.Equals(right);

        Assert.IsFalse(actual);
    }

    [TestMethod]
    [DynamicData(nameof(AreEqualCases))]
    public void Equals_Object_AreEqual_ReturnsTrue(AbsolutePath left, AbsolutePath right)
    {
        bool actual = left.Equals(right);

        Assert.IsTrue(actual);
    }

    [TestMethod]
    [DynamicData(nameof(AreNotEqualCases))]
    public void Equals_Object_AreNotEqual_ReturnsFalse(AbsolutePath? left, AbsolutePath? right)
    {
        if (left is null)
        {
            // Can't invoke the instance method on `null`.
            return;
        }

        bool actual = left.Equals(right);

        Assert.IsFalse(actual);
    }

    [TestMethod]
    [DataRow("foo")]
    [DataRow(@"C:\foo\bar.txt")]
    [DataRow(1)]
    public void Equals_Object_WrongType_ReturnsFalse(object right)
    {
        AbsolutePath instance = new(@"C:\foo\bar.txt");

        bool actual = instance.Equals(right);

        Assert.IsFalse(actual);
    }

    [TestMethod]
    [DynamicData(nameof(AreEqualCases))]
    public void GetHashCode_AreEqual_ReturnsSameValue(AbsolutePath left, AbsolutePath right)
    {
        int leftHash = left.GetHashCode();
        int rightHash = right.GetHashCode();

        Assert.AreEqual(leftHash, rightHash);
    }

    [TestMethod]
    [DataRow(null, null)]
    [DynamicData(nameof(AreEqualCases))]
    public void Operator_Equals_AreEqual_ReturnsTrue(AbsolutePath? left, AbsolutePath? right)
    {
        bool actual = left == right;

        Assert.IsTrue(actual);
    }

    [TestMethod]
    [DynamicData(nameof(AreNotEqualCases))]
    public void Operator_Equals_AreNotEqual_ReturnsFalse(AbsolutePath? left, AbsolutePath? right)
    {
        bool actual = left == right;

        Assert.IsFalse(actual);
    }

    [TestMethod]
    [DynamicData(nameof(GreaterThanCases))]
    public void Operator_GreaterThan_IsGreaterThan_ReturnsTrue(AbsolutePath? left, AbsolutePath? right)
    {
        bool actual = left > right;

        Assert.IsTrue(actual);
    }

    [TestMethod]
    [DynamicData(nameof(AreEqualCases))]
    [DynamicData(nameof(LessThanCases))]
    public void Operator_GreaterThan_IsNotGreaterThan_ReturnsFalse(AbsolutePath? left, AbsolutePath? right)
    {
        bool actual = left > right;

        Assert.IsFalse(actual);
    }

    [TestMethod]
    [DataRow(null, null)]
    [DynamicData(nameof(AreEqualCases))]
    [DynamicData(nameof(GreaterThanCases))]
    public void Operator_GreaterThanOrEqual_IsGreaterThanOrEqual_ReturnsTrue(AbsolutePath? left, AbsolutePath? right)
    {
        bool actual = left >= right;

        Assert.IsTrue(actual);
    }

    [TestMethod]
    [DynamicData(nameof(LessThanCases))]
    public void Operator_GreaterThanOrEqual_IsNotGreaterThanOrEqual_ReturnsFalse(AbsolutePath? left, AbsolutePath? right)
    {
        bool actual = left >= right;

        Assert.IsFalse(actual);
    }

    [TestMethod]
    [DynamicData(nameof(LessThanCases))]
    public void Operator_LessThan_IsLessThan_ReturnsTrue(AbsolutePath? left, AbsolutePath? right)
    {
        bool actual = left < right;

        Assert.IsTrue(actual);
    }

    [TestMethod]
    [DataRow(null, null)]
    [DynamicData(nameof(AreEqualCases))]
    [DynamicData(nameof(GreaterThanCases))]
    public void Operator_LessThan_IsNotLessThan_ReturnsFalse(AbsolutePath? left, AbsolutePath? right)
    {
        bool actual = left < right;

        Assert.IsFalse(actual);
    }

    [TestMethod]
    [DataRow(null, null)]
    [DynamicData(nameof(AreEqualCases))]
    [DynamicData(nameof(LessThanCases))]
    public void Operator_LessThanOrEqual_IsLessThanOrEqual_ReturnsTrue(AbsolutePath? left, AbsolutePath? right)
    {
        bool actual = left <= right;

        Assert.IsTrue(actual);
    }

    [TestMethod]
    [DynamicData(nameof(GreaterThanCases))]
    public void Operator_LessThanOrEqual_IsNotLessThanOrEqual_ReturnsFalse(AbsolutePath? left, AbsolutePath? right)
    {
        bool actual = left <= right;

        Assert.IsFalse(actual);
    }

    [TestMethod]
    [DataRow(null, null)]
    [DynamicData(nameof(AreEqualCases))]
    public void Operator_NotEquals_AreEqual_ReturnsFalse(AbsolutePath? left, AbsolutePath? right)
    {
        bool actual = left != right;

        Assert.IsFalse(actual);
    }

    [TestMethod]
    [DynamicData(nameof(AreNotEqualCases))]
    public void Operator_NotEquals_AreNotEqual_ReturnsTrue(AbsolutePath? left, AbsolutePath? right)
    {
        bool actual = left != right;

        Assert.IsTrue(actual);
    }

    [TestMethod]
    [DynamicData(nameof(AbsolutePaths))]
    public void ToString_Succeeds(string expected)
    {
        AbsolutePath instance = new(expected);

        string actual = instance.ToString();

        Assert.AreEqual(expected, actual);
    }
}