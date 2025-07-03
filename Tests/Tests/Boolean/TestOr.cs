using Amethyst.Utility;
using NUnit.Framework;
using Tests.Utility;
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
        var ctx = Run($"var result = {left.ToNbtString()} || {right.ToNbtString()};");
        var result = ctx.GetResult();
        
        Assert.That(GetAmethystScoreboardValue(result), Is.EqualTo(left || right ? 1 : 0));
    }
    
    [Test]
    [TestCase(false, false)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    [TestCase(true, true)]
    public void TestOrVariable(bool left, bool right)
    {
        var ctx = Run($"""
             var left = {left.ToNbtString()};
             var right = {right.ToNbtString()};
             var result = left || right;
             """);
        var result = ctx.GetResult();
        
        Assert.That(GetAmethystScoreboardValue(result), Is.EqualTo(left || right ? 1 : 0));
    }
    
    [Test]
    [Repeat(5)]
    public void TestOrMultiple()
    {
        var conjuncts = Enumerable.Range(0, 5).Select(_ => _random.Next(2) == 0).ToArray();
        var ctx = Run($"var result = {string.Join(" || ", conjuncts.Select(x => x ? "true" : "false"))};");
        var result = ctx.GetResult();
        
        Assert.That(GetAmethystScoreboardValue(result), Is.EqualTo(conjuncts.Any(x => x) ? 1 : 0));
    }
}