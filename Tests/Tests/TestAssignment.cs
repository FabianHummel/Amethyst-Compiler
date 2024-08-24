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
        
        Assert.That(Scoreboard["amethyst"]["0"], Is.EqualTo(10));
    }
    
    [Test]
    public void TestAssignDecimal()
    {
        Run("""
            var x = 5.24;
            """);
        
        Assert.That(Scoreboard["amethyst"]["0"], Is.EqualTo(524));
    }
    
    [Test]
    public void TestAssignString()
    {
        Run("""
            var x = "Hello, World!";
            """);
        
        // Assert.That(Storage["amethyst:"]["0"], Is.EqualTo("Hello, World!"));
    }
}