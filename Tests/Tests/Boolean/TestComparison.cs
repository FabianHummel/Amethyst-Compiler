using System.Diagnostics;
using Amethyst.Model;
using Amethyst.Utility;
using NUnit.Framework;
using Tests.Utility;
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
        
        var ctx = Run($"var result = {left.ToAmethystString()} {op.GetAmethystOperatorSymbol()} {right.ToAmethystString()};");
        var result = ctx.GetResult();
        
        var expected = op switch
        {
            ComparisonOperator.LESS_THAN => left.ToNbtNumber() < right.ToNbtNumber(),
            ComparisonOperator.LESS_THAN_OR_EQUAL => left.ToNbtNumber() <= right.ToNbtNumber(),
            ComparisonOperator.GREATER_THAN => left.ToNbtNumber() > right.ToNbtNumber(),
            ComparisonOperator.GREATER_THAN_OR_EQUAL => left.ToNbtNumber() >= right.ToNbtNumber(),
            ComparisonOperator.EQUAL => left.ToNbtNumber() == right.ToNbtNumber(),
            ComparisonOperator.NOT_EQUAL => left.ToNbtNumber() != right.ToNbtNumber(),
            _ => throw new UnreachableException() 
        };
        
        Assert.That(GetAmethystScoreboardValue(result), Is.EqualTo(expected ? 1 : 0));
    }
}