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
            var x = [19, "nice", 8.43, [1, "cool", true]];
            """);
     
        Assert.That(GetStorageValue("amethyst:", "0"), Is.EqualTo("[{_: 19}, {_: \"nice\"}, {_: 8.43d}, {_: [{_: 1}, {_: \"cool\"}, {_: 1b}]}]"));
    }
}