using NUnit.Framework;

namespace Tests;

public partial class Program
{
    [Test]
    public void TestFalseOrFalse()
    {
        Run("var result = false || false;");
        Assert.That(GetScoreboardValue("amethyst", "0"), Is.EqualTo(0));
    }
    
    [Test]
    public void TestFalseOrTrue()
    {
        Run("var result = false || true;");
        Assert.That(GetScoreboardValue("amethyst", "0"), Is.EqualTo(1));
    }
    
    [Test]
    public void TestTrueOrFalse()
    {
        Run("var result = true || false;");
        Assert.That(GetScoreboardValue("amethyst", "0"), Is.EqualTo(1));
    }
    
    [Test]
    public void TestTrueOrTrue()
    {
        Run("var result = true || true;");
        Assert.That(GetScoreboardValue("amethyst", "0"), Is.EqualTo(1));
    }
    
    [Test]
    public void TestOrMultipleDisjuncts()
    {
        var disjuncts = Enumerable.Range(0, 10).Select(_ => _random.Next(2) == 0).ToArray();
        Run($"var result = {string.Join(" || ", disjuncts.Select(x => x ? "true" : "false"))};");
        Assert.That(GetScoreboardValue("amethyst", "0"), Is.EqualTo(disjuncts.Any(x => x) ? 1 : 0));
    }
    
    [Test]
    public void TestVariableOrVariable()
    {
        Run("var a = true; var b = false; var result = a || b;");
        Assert.That(GetScoreboardValue("amethyst", "0"), Is.EqualTo(1));
    }
    
    [Test]
    public void TestVariableOrFalse()
    {
        Run("var a = true; var result = a || false;");
        Assert.That(GetScoreboardValue("amethyst", "0"), Is.EqualTo(1));
    }
}