using NUnit.Framework;
using static Tests.TestMain;

namespace Tests;

public class TestAssignment
{
    [Test]
    [Repeat(5)]
    public void TestAssignInt()
    {
        var value = _random.Next(0, 100);
        Run($"var x = {value};");
        Assert.That(GetScoreboardValue("amethyst", "0"), Is.EqualTo(value));
    }
    
    [Test]
    [Repeat(5)]
    public void TestAssignDecimal()
    {
        var value = Math.Truncate(_random.NextDouble() * 100) / 100;
        Run($"var x = {value};");
        Assert.That(GetScoreboardValue("amethyst", "0"), Is.EqualTo((int)(value * 100)));
    }
    
    [Test]
    [TestCase("Hello World!")]
    [TestCase("String with 'single quotes'")]
    [TestCase("String with \"double quotes\"")]
    [TestCase("String with \\backslashes\\")]
    [TestCase("String with \nnewlines\n")]
    public void TestAssignString(string value)
    {
        Run($"var x = \"{value}\";");
        Assert.That(GetStorageValue("amethyst:", "0"), Is.EqualTo($"\"{value}\""));
    }

    [Test]
    [TestCase("[1, 2, 3, 4, 5]", "[1, 2, 3, 4, 5]")]
    [TestCase("[1, 2.5, 3, 4.5, 5]", "[{_: 1}, {_: 2.5d}, {_: 3}, {_: 4.5d}, {_: 5}]")]
    [TestCase("[]", "[]")]
    [TestCase("[[1, 2], [3, 4], [5, 6]]", "[[1, 2], [3, 4], [5, 6]]")]
    [TestCase("[[1, true], [\"hello\", false], [3.5, true]]", "[[{_: 1}, {_: 1b}], [{_: \"hello\"}, {_: 0b}], [{_: 3.5d}, {_: 1b}]]")]
    [TestCase("[19, \"nice\", 8.43, [1, \"cool\", true]]", "[{_: 19}, {_: \"nice\"}, {_: 8.43d}, {_: [{_: 1}, {_: \"cool\"}, {_: 1b}]}]")]
    public void TestAssignArray(string input, string output)
    {
        Run($"var x = {input};");
        Assert.That(GetStorageValue("amethyst:", "0"), Is.EqualTo($"{output}"));
    }
}