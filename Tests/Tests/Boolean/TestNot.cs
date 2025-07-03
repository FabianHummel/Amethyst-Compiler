using Amethyst.Utility;
using NUnit.Framework;
using Tests.Utility;
using static Tests.TestMain;

namespace Tests;

public class TestNot
{
    [Test]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(0, true)]
    [TestCase(1, false)]
    [TestCase("string", false)]
    [TestCase("", true)]
    [TestCase(new object[] { }, true)]
    [TestCase(new object[] { 1 }, false)]
    public void TestNotConstant(object value, bool expected)
    {
        Run($"var result = !{value.ToNbtString()};");
        Assert.That(GetScoreboardValue("amethyst", "0"), Is.EqualTo(expected ? 1 : 0));
    }
    
    [Test]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(0, true)]
    [TestCase(1, false)]
    [TestCase("string", false)]
    [TestCase("", true)]
    [TestCase(new object[] { }, true)]
    [TestCase(new object[] { 1 }, false)]
    public void TestNotVariable(object value, bool expected)
    {
        var ctx = Run($"""
                       var value = {value.ToNbtString()};
                       var result = !value;
                       """);
        var result = ctx.GetResult();
        Assert.That(GetAmethystScoreboardValue(result), Is.EqualTo(expected ? 1 : 0));
    }

    [Test]
    [TestCase(false, false)]
    [TestCase(true, true)]
    [TestCase(0, false)]
    [TestCase(1, true)]
    [TestCase("string", true)]
    [TestCase("", false)]
    [TestCase(new object[] { }, false)]
    [TestCase(new object[] { 1 }, true)]
    public void TestNotMultiple(object value, bool expected)
    {
        var ctx = Run($"var result = !!{value.ToNbtString()};");
        var result = ctx.GetResult();
        Assert.That(GetAmethystScoreboardValue(result), Is.EqualTo(expected ? 1 : 0));
    }
}