using NUnit.Framework;

namespace Tests.Arithmetic;

[TestFixture]
public class RuntimeArithmetic : ServerTestBase
{
    [Test(Description = "Arithmetic with the use of runtime values")]
    [Link("arithmetic/test_addition")]
    public async Task TestAddition(
        [Values(1,-2,3)] int x,
        [Values(1,2,-3)] int y)
    {
        Assert.AreEqual(x + y, await Context.Variable<int>("result"));
    }
}