using Amethyst.Utility;
using NUnit.Framework;
using static Tests.TestMain;

namespace Tests;

public class TestArithmetic
{
    [Test]
    [Repeat(5)]
    public void TestAdd()
    {
        var left = NbtUtility.RandomNbtValue(_random);
        var right = NbtUtility.RandomNbtValue(_random);
        Run($"var result = {left.ToNbtString()} + {right.ToNbtString()};");
        Assert.That(GetScoreboardValue("amethyst", "0"), Is.EqualTo(left.ToNbtNumber() + right.ToNbtNumber()));
    }
    
    [Test]
    [Repeat(5)]
    public void TestSubtract()
    {
        var left = NbtUtility.RandomNbtValue(_random);
        var right = NbtUtility.RandomNbtValue(_random);
        Run($"var result = {left.ToNbtString()} - {right.ToNbtString()};");
        Assert.That(GetScoreboardValue("amethyst", "0"), Is.EqualTo(left.ToNbtNumber() - right.ToNbtNumber()));
    }
    
    [Test]
    [Repeat(5)]
    public void TestMultiply()
    {
        var left = NbtUtility.RandomNbtValue(_random);
        var right = NbtUtility.RandomNbtValue(_random);
        Run($"var result = {left.ToNbtString()} * {right.ToNbtString()};");
        Assert.That(GetScoreboardValue("amethyst", "0"), Is.EqualTo(left.ToNbtNumber() * right.ToNbtNumber()));
    }
    
    [Test]
    [Repeat(5)]
    public void TestDivide()
    {
        var left = NbtUtility.RandomNbtValue(_random);
        var right = NbtUtility.RandomNbtValue(_random);
        Run($"var result = {left.ToNbtString()} / {right.ToNbtString()};");
        Assert.That(GetScoreboardValue("amethyst", "0"), Is.EqualTo(left.ToNbtNumber() / right.ToNbtNumber()));
    }
    
    [Test]
    [Repeat(5)]
    public void TestModulo()
    {
        var left = NbtUtility.RandomNbtValue(_random);
        var right = NbtUtility.RandomNbtValue(_random);
        Run($"var result = {left.ToNbtString()} % {right.ToNbtString()};");
        Assert.That(GetScoreboardValue("amethyst", "0"), Is.EqualTo(left.ToNbtNumber() % right.ToNbtNumber()));
    }
}