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

    private void Execute(Stmt stmt)
    {
        stmt.Accept(this); // don't use return value, as it's always null
    }
    
    private object? Evaluate(Expr expr)
    {
        return expr.Accept(this);
    }
    
    private bool IsTruthy(object? value)
    {
        if (value == null) return false;
        if (value is bool b) return b;
        return true;
    }
    
    private bool IsEqual(object? a, object? b)
    {
        if (a == null && b == null) return true;
        if (a == null) return false;
        return a.Equals(b);
    }
    
    private void CheckNumberOperand(Token op, object? operand)
    {
        if (operand is double) return;
        throw new SyntaxException("Operand must be a number", op.Line);
    }
    
    private void CheckNumberOperands(Token op, object? left, object? right)
    {
        if (left is double && right is double) return;
        throw new SyntaxException("Operands must be numbers", op.Line);
    }

    public object? VisitAssignExpr(Expr.Assign expr)
    {
        return null;
    }

    public object? VisitBinaryExpr(Expr.Binary expr)
    {
        object? left = Evaluate(expr.Left);
        object? right = Evaluate(expr.Right);

        switch (expr.Operator.Type)
        {
            case TokenType.GREATER:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left! > (double)right!;
            case TokenType.GREATER_EQUAL:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left! >= (double)right!;
            case TokenType.LESS:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left! < (double)right!;
            case TokenType.LESS_EQUAL:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left! <= (double)right!;
            case TokenType.MINUS:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left! - (double)right!;
            case TokenType.PLUS:
                if (left is double && right is double)
                {
                    return (double)left! + (double)right!;
                }
                if (left is string && right is string)
                {
                    return (string)left! + (string)right!;
                }
                break;
            case TokenType.SLASH:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left! / (double)right!;
            case TokenType.STAR:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left! * (double)right!;
            case TokenType.MODULO:
                CheckNumberOperands(expr.Operator, left, right);
                return (double)left! % (double)right!;
            case TokenType.BANG_EQUAL:
                return !IsEqual(left, right);
            case TokenType.EQUAL_EQUAL:
                return IsEqual(left, right);
        }

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

    public object? VisitLogicalExpr(Expr.Logical expr)
    {
        return null;
    }

    public object? VisitUnaryExpr(Expr.Unary expr)
    {
        object? right = Evaluate(expr.Right);

        switch (expr.Operator.Type)
        {
            case TokenType.BANG:
                return !IsTruthy(right);
            case TokenType.MINUS:
                CheckNumberOperand(expr.Operator, right);
                return -(double)right!;
        }
        
        return null;
    }

    public object? VisitVariableExpr(Expr.Variable expr)
    {
        return null;
    }
    
    private void ExecuteBlock(Stmt.Block block, Environment environment)
    {
        var previous = Environment;
        try
        {
            Environment = environment;
            foreach (var statement in block.Statements)
            {
                Execute(statement);
            }
        }
        finally
        {
            Environment = previous;
        }
    }

    public object? VisitBlockStmt(Stmt.Block stmt)
    {
        ExecuteBlock(stmt, new Environment(Environment));
        return null;
    }

    public object? VisitExpressionStmt(Stmt.Expression stmt)
    {
        Evaluate(stmt.Expr);
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

    public object? VisitReturnStmt(Stmt.Return stmt)
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
            Execute(statement);
        }
    }
}