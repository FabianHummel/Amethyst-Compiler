using NUnit.Framework;

namespace Tests.Arithmetic;

[TestFixture]
public class RuntimeArithmetic : ServerTestBase
{
    [Test(Description = "Addition between two integers")]
    [Link("arithmetic/integer/test_addition")]
    public async Task TestAddition(
        [Values(1,-2,3,-4)] int x,
        [Values(6,7,-8,-9)] int y)
    {
        Assert.AreEqual(x + y, await Context.Variable<int>("result"));
    }
    
    [Test(Description = "Subtraction between two integers")]
    [Link("arithmetic/integer/test_subtraction")]
    public async Task TestSubtraction(
        [Values(1,-2,3,-4)] int x,
        [Values(6,7,-8,-9)] int y)
    {
        Assert.AreEqual(x - y, await Context.Variable<int>("result"));
    }
    
    [Test(Description = "Multiplication between two integers")]
    [Link("arithmetic/integer/test_multiplication")]
    public async Task TestMultiplication(
        [Values(1,-2,3,-4)] int x,
        [Values(6,7,-8,-9)] int y)
    {
        Assert.AreEqual(x * y, await Context.Variable<int>("result"));
    }
    
    [Test(Description = "Division between two integers")]
    [Link("arithmetic/integer/test_division")]
    public async Task TestDivision(
        [Values(1,-2,3,-4)] int x,
        [Values(6,7,-8,-9)] int y)
    {
        Assert.AreEqual(x / y, await Context.Variable<int>("result"));
    }
    
    [Test(Description = "Modulo between two integers")]
    [Link("arithmetic/integer/test_modulo")]
    public async Task TestModulo(
        [Values(1,-2,3,-4)] int x,
        [Values(6,7,-8,-9)] int y)
    {
        Assert.AreEqual(Modulo(x, y), await Context.Variable<int>("result"));
    }
}