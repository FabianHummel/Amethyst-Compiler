using Amethyst.Utility;
using NUnit.Framework;
using Tests.Utility;
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
        var ctx = Run($"var result = {left.ToNbtString()} + {right.ToNbtString()};");
        var result = ctx.GetResult();
        
        Assert.That(GetAmethystScoreboardValue(result), Is.EqualTo(left.ToNbtNumber() + right.ToNbtNumber()));
    }
    
    [Test]
    [Repeat(5)]
    public void TestSubtract()
    {
        var left = NbtUtility.RandomNbtValue(_random);
        var right = NbtUtility.RandomNbtValue(_random);
        var ctx = Run($"var result = {left.ToNbtString()} - {right.ToNbtString()};");
        var result = ctx.GetResult();
        
        Assert.That(GetAmethystScoreboardValue(result), Is.EqualTo(left.ToNbtNumber() - right.ToNbtNumber()));
    }
    
    [Test]
    [Repeat(5)]
    public void TestMultiply()
    {
        var left = NbtUtility.RandomNbtValue(_random);
        var right = NbtUtility.RandomNbtValue(_random);
        var ctx = Run($"var result = {left.ToNbtString()} * {right.ToNbtString()};");
        var result = ctx.GetResult();
        
        Assert.That(GetAmethystScoreboardValue(result), Is.EqualTo(left.ToNbtNumber() * right.ToNbtNumber()));
    }
    
    [Test]
    [Repeat(5)]
    public void TestDivide()
    {
        var left = NbtUtility.RandomNbtValue(_random);
        var right = NbtUtility.RandomNbtValue(_random);
        var ctx = Run($"var result = {left.ToNbtString()} / {right.ToNbtString()};");
        var result = ctx.GetResult();
        
        Assert.That(GetAmethystScoreboardValue(result), Is.EqualTo(left.ToNbtNumber() / right.ToNbtNumber()));
    }
    
    [Test]
    [Repeat(5)]
    public void TestModulo()
    {
        var left = NbtUtility.RandomNbtValue(_random);
        var right = NbtUtility.RandomNbtValue(_random);
        var ctx = Run($"var result = {left.ToNbtString()} % {right.ToNbtString()};");
        var result = ctx.GetResult();
        
        Assert.That(GetAmethystScoreboardValue(result), Is.EqualTo(left.ToNbtNumber() % right.ToNbtNumber()));
    }
}