using NUnit.Framework;

namespace Tests;

public partial class Program
{
    [Test]
    public void TestAssignInt()
    {
        Run("""
            var x = 10;
            """);

        Assert.That(GetScoreboardValue("amethyst", "0"), Is.EqualTo(10));
    }
    
    [Test]
    public void TestAssignDecimal()
    {
        Run("""
            var x = 5.24;
            """);
     
        Assert.That(GetScoreboardValue("amethyst", "0"), Is.EqualTo(524));
    }
    
    [Test]
    public void TestAssignString()
    {
        Run("""
            var x = "Hello, World!";
            """);

        Assert.That(GetStorageValue("amethyst:", "0"), Is.EqualTo("\"Hello, World!\""));
    }
    
    [Test]
    public void TestAssignArray()
    {
        Run("""
            var x = [20, 30, 4, 5];
            """);
     
        Assert.That(GetStorageValue("amethyst:", "0"), Is.EqualTo("[20,30,4,5]"));
    }
}