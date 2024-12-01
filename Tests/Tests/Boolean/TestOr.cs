using Amethyst.Utility;
using NUnit.Framework;
using static Tests.TestMain;

namespace Tests;

public class TestOr
{
    [Test]
    [TestCase(false, false)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(true, true)]
    public void TestOrConstant(bool left, bool right)
    {
        Run($"var result = {left.ToNbtString()} || {right.ToNbtString()};");
        Assert.That(GetScoreboardValue("amethyst", "0"), Is.EqualTo(left || right ? 1 : 0));
    }
    
    [Test]
    [TestCase(false, false)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(true, true)]
    public void TestOrVariable(bool left, bool right)
    {
        Run($"""
             var left = {left.ToNbtString()};
             var right = {right.ToNbtString()};
             var result = left || right;
             """);
        Assert.That(GetScoreboardValue("amethyst", "0"), Is.EqualTo(left || right ? 1 : 0));
    }
    
    [Test]
    [Repeat(5)]
    public void TestOrMultiple()
    {
        var conjuncts = Enumerable.Range(0, 5).Select(_ => _random.Next(2) == 0).ToArray();
        Run($"var result = {string.Join(" || ", conjuncts.Select(x => x ? "true" : "false"))};");
        Assert.That(GetScoreboardValue("amethyst", "0"), Is.EqualTo(conjuncts.Any(x => x) ? 1 : 0));
    }
}