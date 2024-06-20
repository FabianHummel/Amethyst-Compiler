using Amethyst.Model;

namespace Amethyst.Utility;

public static class PipelineExtensions
{
    public static IEnumerable<Token> Tokenize(this string input, Namespace context)
    {
        return new Tokenizer(input, context).Tokenize();
    }
    
    public static Stmt Parse(this IEnumerable<Token> tokens, Namespace context)
    {
        return new Parser.Parser(tokens.ToList(), context).Parse();
    }
    
    public static void Compile(this IEnumerable<Stmt> stmts, Context context)
    {
        new Compiler.Compiler(stmts.ToList(), context).Compile();
    }
}