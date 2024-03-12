using System.Text;

namespace Amethyst;

public static class Generator
{
    public static string Generate(IEnumerable<AstNode> nodes)
    {
        var sb = new StringBuilder();
        var context = new AstContext();
        foreach (var node in nodes)
        {
            node.ToCode(context);
        }
        return sb.ToString();
    }
}