using Amethyst.Model;
using NUnit.Framework;

namespace Tests;

[TestFixture]
public class LoadFunctionsComparerTests
{
    private static IEnumerable<TestCaseData> TestCases =
    [
        new TestCaseData(new List<string>
            {
                "aaa:abc", "zzz:aaa", "amethyst:_init", "amethyst:abc", "zzz:abc", "aaa:_init", "zzz:_init"
            })
            .Returns(new[]
            {
                "amethyst:_init", "amethyst:abc", "aaa:_init", "aaa:abc", "zzz:_init", "zzz:aaa", "zzz:abc"
            }),
        
        new TestCaseData(new List<string>
            {
                "amethyst:_abc", "amethyst:_init", "amethyst:abc"
            })
            .Returns(new[]
            {
                "amethyst:_init", "amethyst:_abc", "amethyst:abc"
            })
    ];
    
    [Test, TestCaseSource(nameof(TestCases))]
    public IEnumerable<string> TestLoadFunctionsComparer(IEnumerable<string> functionNames)
    {
        // Arrange
        var comparer = new Datapack.LoadFunctionsComparer();
        var set = new SortedSet<string>(functionNames, comparer);
        return set.ToArray();
    }
}