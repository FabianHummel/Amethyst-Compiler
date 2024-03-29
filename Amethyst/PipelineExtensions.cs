namespace Amethyst;

public static class PipelineExtensions
{
    public static IList<Token> Tokenize(this string input)
    {
        return new Tokenizer(input).Tokenize();
    }
    
    public static IList<Stmt> Parse(this IList<Token> tokens)
    {
        return new Parser(tokens).Parse();
    }
    
    public static IList<Stmt> Optimize(this IList<Stmt> stmts)
    {
        return new Optimizer(stmts).Optimize();
    }
    
    public static IList<Stmt> Preprocess(this IList<Stmt> stmts)
    {
        return new Preprocessor(stmts).Preprocess();
    }
    
    public static void Compile(this IList<Stmt> stmts, string rootNamespace, string outDir)
    {
        new Compiler(stmts, rootNamespace, outDir).Compile();
    }
}