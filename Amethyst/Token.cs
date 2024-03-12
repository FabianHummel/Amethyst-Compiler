namespace Amethyst;

public class Token
{
    public required TokenType Type { get; set; }
    public required string Lexeme { get; set; }
    public required int Line { get; set; }

    public override string ToString()
    {
        return $"{Type} {Lexeme}";
    }
}