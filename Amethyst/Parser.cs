namespace Amethyst;

public static class Parser
{
    public static void Parse(List<(TokenType type, string info)> input)
    {
        int i = 0;
        while (i < input.Count && input.Count > 0)
        {
            var token = input[i];
            
            if (token.type is TokenType.KWD_FUNCTION or TokenType.KWD_TICKING or TokenType.KWD_INITIALIZING)
            {
                var function = Function.Consume(input);
                Console.Out.WriteLine("Parsed function " + function.Name);
            }
            
            i++;
        }
    }
}