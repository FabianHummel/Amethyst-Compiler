using NUnit.Framework;

namespace Tests.Arithmetic;

[TestFixture]
public class RuntimeArithmetic
{
    [Test, Description("Arithmetic with the use of runtime values")]
    [AmethystProject(Path = "Assets/Arithmetic/RuntimeArithmetic")]
    public void Test()
    {
        Assert.Pass();
    }
}