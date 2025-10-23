using Amethyst;
using NUnit.Framework;
using Tests.Presets;
using Tests.Providers;

namespace Tests;

[TestFixture]
public class ArithmeticTests : ServerTestBase
{
    [Test(Description = "Arithmetic between two integers.")]
    [Link("arithmetic/test_integer_{arithmeticOperatorTestCase.Name}")]
    public async Task TestInteger(
        [Random(-100, 100, count: 4), FunctionParameter] int x,
        [Random(-100, 100, count: 4), FunctionParameter] int y,
        [ValueSource(typeof(ArithmeticOperatorTestCase), nameof(ArithmeticOperatorTestCase.Preset))] 
        ArithmeticOperatorTestCase arithmeticOperatorTestCase)
    {
        if (arithmeticOperatorTestCase.Operator == "/" && y == 0)
            y = 1; // Avoid division by zero
        
        double unrounded = arithmeticOperatorTestCase.Operator switch
        {
            "+" => x + y,
            "-" => x - y,
            "*" => x * y,
            "/" => (double)x / y,
            "%" => Modulo(x, y),
            _ => throw new NotImplementedException()
        };
        
        var expected = (int)Math.Floor(unrounded);
        
        Assert.AreEqual(expected, await Context.Variable<int>("result"));
    }
    
    [Test(Description = "Arithmetic between two decimals."), Pairwise]
    [Link("arithmetic/test_decimal_{arithmeticOperatorTestCase.Name}")]
    public async Task TestDecimal(
        [Values(1.23, 24.58, 72.97, -16.93, 1736.12), FunctionParameter] double x,
        [Values(-21.31, 24.57, 15.21, -956.12, 38.03), FunctionParameter] double y,
        [ValueSource(typeof(ArithmeticOperatorTestCase), nameof(ArithmeticOperatorTestCase.Preset))] 
        ArithmeticOperatorTestCase arithmeticOperatorTestCase)
    {
        var unrounded = arithmeticOperatorTestCase.Operator switch
        {
            "+" => x + y,
            "-" => x - y,
            "*" => x * y,
            "/" => x / y,
            "%" => Modulo(x, y),
            _ => throw new NotImplementedException()
        };

        var actual = await Context.Variable<double>("result", out var variable);
        var expected = CalculateRounded(unrounded, (DecimalDatatype)variable.Datatype, out var tolerance);
        
        Assert.AreEqual(expected, actual, tolerance * 2);
    }
    
    [Test(Description = "Arithmetic between a decimal with two decimal places and an integer."), Pairwise]
    [Link("arithmetic/test_decimal2_integer_{arithmeticOperatorTestCase.Name}")]
    public async Task TestDecimal2AndInteger(
        [Values(-21.31, 24.57, 15.21, -956.12, 38.03), FunctionParameter] double x,
        [Values(1, 24, 72, -16, 1736), FunctionParameter] int y,
        [ValueSource(typeof(ArithmeticOperatorTestCase), nameof(ArithmeticOperatorTestCase.Preset))] 
        ArithmeticOperatorTestCase arithmeticOperatorTestCase)
    {
        var unrounded = arithmeticOperatorTestCase.Operator switch
        {
            "+" => x + y,
            "-" => x - y,
            "*" => x * y,
            "/" => x / y,
            "%" => Modulo(x, y),
            _ => throw new NotImplementedException()
        };

        var actual = await Context.Variable<double>("result", out var variable);
        var expected = CalculateRounded(unrounded, (DecimalDatatype)variable.Datatype, out var tolerance);
        
        Assert.AreEqual(expected, actual, tolerance * 2);
    }
    
    [Test(Description = "Arithmetic between a boolean and an integer."), Pairwise]
    [Link("arithmetic/test_boolean_integer_{arithmeticOperatorTestCase.Name}")]
    public async Task TestBooleanAndInteger(
        [Values(true, false), FunctionParameter] bool x,
        [Values(-21, 24, 15, -956, 38), FunctionParameter] int y,
        [ValueSource(typeof(ArithmeticOperatorTestCase), nameof(ArithmeticOperatorTestCase.Preset))] 
        ArithmeticOperatorTestCase arithmeticOperatorTestCase)
    {
        var xI = x ? 1.0 : 0.0;
        
        var unrounded = arithmeticOperatorTestCase.Operator switch
        {
            "+" => xI + y,
            "-" => xI - y,
            "*" => xI * y,
            "/" => xI / y,
            "%" => Modulo(xI, y),
            _ => throw new NotImplementedException()
        };

        var actual = await Context.Variable<int>("result", out var variable);
        var expected = (int)Math.Floor(unrounded);
        
        Assert.AreEqual(expected, actual);
    }
    
    [Test(Description = "Arithmetic between two decimals with three and one decimal places."), Pairwise]
    [Link("arithmetic/test_decimal3_decimal1_{arithmeticOperatorTestCase.Name}")]
    public async Task TestDecimal3AndDecimal1(
        [Values(1.231, 24.587, 72.971, -16.932, 1736.123), FunctionParameter] double x,
        [Values(-21.3, 24.5, 15.2, -956.1, 38.7), FunctionParameter] double y,
        [ValueSource(typeof(ArithmeticOperatorTestCase), nameof(ArithmeticOperatorTestCase.Preset))] 
        ArithmeticOperatorTestCase arithmeticOperatorTestCase)
    {
        var unrounded = arithmeticOperatorTestCase.Operator switch
        {
            "+" => x + y,
            "-" => x - y,
            "*" => x * y,
            "/" => x / y,
            "%" => Modulo(x, y),
            _ => throw new NotImplementedException()
        };

        var actual = await Context.Variable<double>("result", out var variable);
        var expected = CalculateRounded(unrounded, (DecimalDatatype)variable.Datatype, out var tolerance);
        
        Assert.AreEqual(expected, actual, tolerance * 2);
    }
    
    private static double CalculateRounded(double value, DecimalDatatype dataType, out double tolerance)
    {
        tolerance = 1.0 / dataType.Scale;
        if (dataType.Scale == 1) tolerance = 0.0; // No tolerance for whole numbers
        return Math.Round(value, digits: dataType.DecimalPlaces, MidpointRounding.ToNegativeInfinity);
    }
}