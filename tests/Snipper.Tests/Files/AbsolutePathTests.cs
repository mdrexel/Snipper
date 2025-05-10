using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Snipper.Tests.Files;

[TestClass]
public sealed class AbsolutePathTests
{
    [TestMethod]
    [DataRow(@"C:\")]
    [DataRow(@"C:\foo")]
    [DataRow(@"C:\foo\bar.txt")]
    [DataRow(@"C:\foo\bar\")]
    [DataRow(@"C:\foo\bar\baz.txt")]
    [DataRow(@"X:\")]
    [DataRow(@"X:\qux\corge")]
    [DataRow(@"X:\qux\corge\grault.txt")]
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

    // Because `AbsolutePath` is `abstract`, we need to derive a type from it to test it in isolation.
    private sealed class AbsolutePath(string path) : Snipper.Files.AbsolutePath(path);
}