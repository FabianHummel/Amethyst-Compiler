using Amethyst.Model;
using Amethyst.Utility;
using NUnit.Framework;
using static Tests.TestMain;

namespace Tests;

public class TestComparison
{
    [Test]
    [Repeat(5)]
    [TestCase(ComparisonOperator.LESS_THAN)]
    [TestCase(ComparisonOperator.LESS_THAN_OR_EQUAL)]
    [TestCase(ComparisonOperator.GREATER_THAN)]
    [TestCase(ComparisonOperator.GREATER_THAN_OR_EQUAL)]
    [TestCase(ComparisonOperator.EQUAL)]
    [TestCase(ComparisonOperator.NOT_EQUAL)]
    public void TestComparisonConstant(ComparisonOperator op)
    {
        var left = NbtUtility.RandomNbtValue(_random);
        var right = NbtUtility.RandomNbtValue(_random);
        
        Run($"var result = {left.ToNbtString()} {op.GetAmethystOperatorSymbol()} {right.ToNbtString()};");
        
        var expected = op switch
        {
            ComparisonOperator.LESS_THAN => left.ToNbtNumber() < right.ToNbtNumber(),
            ComparisonOperator.LESS_THAN_OR_EQUAL => left.ToNbtNumber() <= right.ToNbtNumber(),
            ComparisonOperator.GREATER_THAN => left.ToNbtNumber() > right.ToNbtNumber(),
            ComparisonOperator.GREATER_THAN_OR_EQUAL => left.ToNbtNumber() >= right.ToNbtNumber(),
            ComparisonOperator.EQUAL => left.ToNbtNumber() == right.ToNbtNumber(),
            ComparisonOperator.NOT_EQUAL => left.ToNbtNumber() != right.ToNbtNumber(),
            _ => throw new ArgumentOutOfRangeException() 
        };
        
        Assert.That(GetScoreboardValue("amethyst", "0"), Is.EqualTo(expected ? 1 : 0));
    }
    
    [Test]
    [Repeat(5)]
    [TestCase(ComparisonOperator.LESS_THAN)]
    [TestCase(ComparisonOperator.LESS_THAN_OR_EQUAL)]
    [TestCase(ComparisonOperator.GREATER_THAN)]
    [TestCase(ComparisonOperator.GREATER_THAN_OR_EQUAL)]
    [TestCase(ComparisonOperator.EQUAL)]
    [TestCase(ComparisonOperator.NOT_EQUAL)]
    public void TestComparisonVariable(ComparisonOperator op)
    {
        var left = NbtUtility.RandomNbtValue(_random);
        var right = NbtUtility.RandomNbtValue(_random);
        
        Run($"""
             var left = {left.ToNbtString()};
             var right = {right.ToNbtString()};
             var result = left || right;
             """);
        
        var expected = op switch
        {
            ComparisonOperator.LESS_THAN => left.ToNbtNumber() < right.ToNbtNumber(),
            ComparisonOperator.LESS_THAN_OR_EQUAL => left.ToNbtNumber() <= right.ToNbtNumber(),
            ComparisonOperator.GREATER_THAN => left.ToNbtNumber() > right.ToNbtNumber(),
            ComparisonOperator.GREATER_THAN_OR_EQUAL => left.ToNbtNumber() >= right.ToNbtNumber(),
            ComparisonOperator.EQUAL => left.ToNbtNumber() == right.ToNbtNumber(),
            ComparisonOperator.NOT_EQUAL => left.ToNbtNumber() != right.ToNbtNumber(),
            _ => throw new ArgumentOutOfRangeException() 
        };
        
        Assert.That(GetScoreboardValue("amethyst", "0"), Is.EqualTo(expected ? 1 : 0));
    }

    [Test]
    [TestCase(ComparisonOperator.LESS_THAN, 1, 2, 3, 4, 5)]
    public void TestComparisonMultiple(ComparisonOperator op, params object[] values)
    {
        var nbtValues = values.Select(NbtUtility.ToNbtNumber).ToArray();
        
        Run($"var result = {string.Join($" {op.GetAmethystOperatorSymbol()} ", nbtValues.Select(value => value.ToNbtString()))};");
        
        var expected = op switch
        {
            ComparisonOperator.LESS_THAN => nbtValues.Skip(1).All(value => nbtValues[0].ToNbtNumber() < value.ToNbtNumber()),
            ComparisonOperator.LESS_THAN_OR_EQUAL => nbtValues.Skip(1).All(value => nbtValues[0].ToNbtNumber() <= value.ToNbtNumber()),
            ComparisonOperator.GREATER_THAN => nbtValues.Skip(1).All(value => nbtValues[0].ToNbtNumber() > value.ToNbtNumber()),
            ComparisonOperator.GREATER_THAN_OR_EQUAL => nbtValues.Skip(1).All(value => nbtValues[0].ToNbtNumber() >= value.ToNbtNumber()),
            ComparisonOperator.EQUAL => nbtValues.Skip(1).All(value => nbtValues[0].ToNbtNumber() == value.ToNbtNumber()),
            ComparisonOperator.NOT_EQUAL => nbtValues.Skip(1).All(value => nbtValues[0].ToNbtNumber() != value.ToNbtNumber()),
            _ => throw new ArgumentOutOfRangeException() 
        };
        
        Assert.That(GetScoreboardValue("amethyst", "0"), Is.EqualTo(expected ? 1 : 0));
    }
}