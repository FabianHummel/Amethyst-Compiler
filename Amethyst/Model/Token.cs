namespace Amethyst.Model;

public class Token
{
    public required TokenType Type { get; init; }
    public required string Lexeme { get; init; }
    public required object Literal { get; init; }
    public required int Line { get; init; }

    public override string ToString()
    {
        return $"{Lexeme}";
    }
}