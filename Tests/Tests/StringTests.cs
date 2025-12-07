using NUnit.Framework;
using Tests.Presets;

namespace Tests;

[TestFixture]
public class StringTests : ServerTestBase
{
    [Test(Description = "Regular strings")]
    [Link("strings/test_regular_{stringTestCase.Name}")]
    public async Task TestRegular(
        [ValueSource(typeof(StringTestCase), nameof(StringTestCase.Regular))] 
        StringTestCase stringTestCase)
    {
        Assert.AreEqual(stringTestCase.Value, await Context.Variable<string>("result"));
    }

    [Test(Description = "Interpolated strings")]
    [Link("strings/test_interpolated_{stringTestCase.Name}")]
    public async Task TestInterpolated(
        [ValueSource(typeof(StringTestCase), nameof(StringTestCase.Interpolated))] 
        StringTestCase stringTestCase)
    {
        Assert.AreEqual(stringTestCase.Expected, await Context.Variable<string>("result"));
    }
}