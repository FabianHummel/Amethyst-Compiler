using NUnit.Framework;

namespace Tests;

public partial class Program
{
    [Test]
    public void TestIntegerPlusInteger()
    {
        var augend = _random.Next(0, 100);
        var addend = _random.Next(0, 100);
        Run($"var sum = {augend} + {addend};");
        Assert.That(GetScoreboardValue("amethyst", "0"), Is.EqualTo(augend + addend));
    }
    
    [Test]
    public void TestIntegerPlusDecimal()
    {
        var augend = _random.Next(0, 100);
        var addend = Math.Truncate(_random.NextDouble() * 100) / 100;
        Run($"var sum = {augend} + {addend};");
        Assert.That(GetScoreboardValue("amethyst", "0"), Is.EqualTo(Math.Truncate((augend + addend) * 100)));
    }
    
    [Test]
    public void TestIntegerMinusBoolean()
    {
        var minuend = _random.Next(0, 100);
        var subtrahend = _random.Next(0, 2);
        Run($"var difference = {minuend} - {(subtrahend == 1 ? "true" : "false")};");
        Assert.That(GetScoreboardValue("amethyst", "0"), Is.EqualTo(minuend - subtrahend));
    }
}