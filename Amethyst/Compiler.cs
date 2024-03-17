using Tommy;

namespace Amethyst;

public class Compiler : Expr.IVisitor<object?>, Stmt.IVisitor<object?>
{
    private IList<Stmt> Statements { get; }
    private string RootNamespace { get; }
    private string OutDir { get; }
    private Environment Environment { get; set; } = new();
    
    public Compiler(IList<Stmt> statements, string rootNamespace, string outDir)
    {
        Statements = statements;
        RootNamespace = rootNamespace;
        OutDir = outDir;
    }

    private void Compile(Stmt stmt)
    {
        stmt.Accept(this); // don't use return value, as it's always null
    }
    
    private object? Evaluate(Expr expr)
    {
        return expr.Accept(this);
    }

    public object? VisitAssignExpr(Expr.Assign expr)
    {
        return null;
    }

    public object? VisitBinaryExpr(Expr.Binary expr)
    {
        return null;
    }

    public object? VisitCallExpr(Expr.Call expr)
    {
        return null;
    }

    public object? VisitGroupingExpr(Expr.Grouping expr)
    {
        return Evaluate(expr.Expression);
    }

    public object? VisitLiteralExpr(Expr.Literal expr)
    {
        return expr.Value;
    }

    public object? VisitObjectExpr(Expr.Object expr)
    {
        return null;
    }

    public object? VisitArrayExpr(Expr.Array expr)
    {
        return null;
    }

    public object? VisitLogicalExpr(Expr.Logical expr)
    {
        return null;
    }

    public object? VisitUnaryExpr(Expr.Unary expr)
    {
        return null;
    }

    public object? VisitVariableExpr(Expr.Variable expr)
    {
        return null;
    }
    
    private void CompileBlock(Stmt.Block block, Environment environment)
    {
        var previous = Environment;
        Environment = environment;
        foreach (var statement in block.Statements)
        {
            Compile(statement);
        }
        Environment = previous;
    }

    public object? VisitBlockStmt(Stmt.Block stmt)
    {
        CompileBlock(stmt, new Environment(Environment));
        return null;
    }

    public object? VisitExpressionStmt(Stmt.Expression stmt)
    {
        Evaluate(stmt.Expr);
        return null;
    }

    public object? VisitNamespaceStmt(Stmt.Namespace stmt)
    {
        return null;
    }

    public object? VisitFunctionStmt(Stmt.Function stmt)
    {
        return null;
    }

    public object? VisitIfStmt(Stmt.If stmt)
    {
        return null;
    }

    public object? VisitPrintStmt(Stmt.Print stmt)
    {
        return null;
    }

    public object? VisitOutStmt(Stmt.Out stmt)
    {
        return null;
    }

    public object? VisitVarStmt(Stmt.Var stmt)
    {
        return null;
    }

    public object? VisitWhileStmt(Stmt.While stmt)
    {
        return null;
    }

    public void Compile()
    {
        foreach (var statement in Statements)
        {
            Compile(statement);
        }
    }
}