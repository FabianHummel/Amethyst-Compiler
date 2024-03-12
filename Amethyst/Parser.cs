namespace Amethyst;

public static class Parser
{
    public static IEnumerable<AstNode> Parse(IList<(TokenType type, string info)> input)
    {
        var nodes = new List<AstNode>();
        while (input.Count > 0)
        {
            if (input[0].type is TokenType.KWD_VARIABLE)
            {
                nodes.Add(Variable.Consume(input));
            }
            else if (input[0].type is TokenType.KWD_FUNCTION or TokenType.KWD_TICKING or TokenType.KWD_INITIALIZING)
            {
                nodes.Add(Function.Consume(input));
            }
            else if (input[0].type is TokenType.KWD_NAMESPACE)
            {
                nodes.Add(Namespace.Consume(input));
            }
            else
            {
                // something could not be parsed (just skip it)
                input.RemoveAt(0);
            }
        }
        return nodes;
    }
}