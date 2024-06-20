using Amethyst.Framework;
using Amethyst.Model;

namespace Amethyst.Utility;

public static class PipelineExtensions
{
    public static IList<Token> Tokenize(this string input, string sourceFile)
    {
        return new Tokenizer(input, sourceFile).Tokenize();
    }
    
    public static IList<Stmt> Parse(this IList<Token> tokens, string sourceFile)
    {
        return new Parser(tokens, sourceFile).Parse();
    }
    
    public static IList<Stmt> Optimize(this IList<Stmt> stmts)
    {
        return new Optimizer(stmts).Optimize();
    }
    
    public static IList<Stmt> Preprocess(this IList<Stmt> stmts)
    {
        return new Preprocessor(stmts).Preprocess();
    }
    
    public static void Compile(this IList<Stmt> stmts, string rootNamespace, string outDir, string sourceFile)
    {
        new Compiler(stmts, rootNamespace, outDir, sourceFile).Compile();
    }
}