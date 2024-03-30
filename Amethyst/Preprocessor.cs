using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Amethyst;

public class Preprocessor : Stmt.IVisitor<Stmt?>, Expr.IVisitor<object>
{
    private class Environment
    {
        private Dictionary<string, object> Values { get; } = new();
        
        public void Define(string name, object value)
        {
            Values[name] = value;
        }

        public bool TryGet(string name, [NotNullWhen(true)] out object? value)
        {
            return Values.TryGetValue(name, out value);
        }
        
        public void Assign(string name, object value)
        {
            if (Values.ContainsKey(name))
            {
                Values[name] = value;
            }
        }
    }
    
    private IList<Stmt> Statements { get; }
    private Environment Scope { get; }
    
    public Preprocessor(IList<Stmt> stmts)
    {
        Statements = stmts;
        Scope = new Environment();
    }
    
    public IList<Stmt> Preprocess()
    {
        try
        {
            return PreprocessBlock(Statements);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"Preprocessor error: {e.Message}");
            return new List<Stmt>();
        }
    }
    
    private IList<Stmt> PreprocessBlock(IEnumerable<Stmt> statements)
    {
        var stmts = new List<Stmt>();
        
        foreach (var statement in statements)
        {
            if (statement.Accept(this) is { } stmt)
            {
                stmts.Add(stmt);
            }
        }
        
        return stmts;
    }

    public Stmt VisitBlockStmt(Stmt.Block stmt)
    {
        return new Stmt.Block
        {
            Statements = PreprocessBlock(stmt.Statements)
        };
    }

    public Stmt VisitExpressionStmt(Stmt.Expression stmt)
    {
        return stmt.Expr.Accept(this) switch
        {
            null => null!,
            _ => stmt
        };
    }

    public Stmt VisitNamespaceStmt(Stmt.Namespace stmt)
    {
        if (!stmt.IsPreprocessed) return stmt;
        
        // todo: add namespace to the scope
        
        return new Stmt.Namespace
        {
            Name = stmt.Name,
            Body = PreprocessBlock(stmt.Body),
            IsPreprocessed = true
        };
    }

    public Stmt VisitFunctionStmt(Stmt.Function stmt)
    {
        if (!stmt.IsPreprocessed) return stmt;
        
        // todo: add function to the scope
        
        return new Stmt.Function
        {
            Name = stmt.Name,
            Body = PreprocessBlock(stmt.Body),
            Params = stmt.Params,
            Initializing = stmt.Initializing,
            Ticking = stmt.Ticking,
            IsPreprocessed = true
        };
    }

    public Stmt? VisitIfStmt(Stmt.If stmt)
    {
        if (!stmt.IsPreprocessed) return stmt;
        
        if (stmt.Condition.Accept(this) is not bool condition)
        {
            throw new Exception("If statement condition must be a constant boolean expression.");
        }
        
        if (condition)
        {
            return stmt.ThenBranch.Accept(this);
        }
        
        if (stmt.ElseBranch != null)
        {
            return stmt.ElseBranch.Accept(this);
        }

        return stmt;
    }

    public Stmt? VisitPrintStmt(Stmt.Print stmt)
    {
        if (!stmt.IsPreprocessed) return new Stmt.Print
        {
            Expr = new Expr.Literal
            {
                Value = stmt.Expr.Accept(this)
            },
            IsPreprocessed = true
        };

        Console.Out.WriteLine(stmt.Expr.Accept(this));

        return null;
    }

    public Stmt VisitCommentStmt(Stmt.Comment stmt)
    {
        return stmt;
    }

    public Stmt VisitReturnStmt(Stmt.Return stmt)
    {
        if (!stmt.IsPreprocessed) return stmt;
        
        return stmt;
    }

    public Stmt VisitBreakStmt(Stmt.Break stmt)
    {
        if (!stmt.IsPreprocessed) return stmt;
        
        return stmt;
    }

    public Stmt VisitContinueStmt(Stmt.Continue stmt)
    {
        if (!stmt.IsPreprocessed) return stmt;
        
        return stmt;
    }

    public Stmt? VisitVarStmt(Stmt.Var stmt)
    {
        if (!stmt.IsPreprocessed) return stmt;
        
        if (stmt.Initializer != null)
        {
            Scope.Define(stmt.Name.Lexeme, stmt.Initializer.Accept(this));
        }

        return null;
    }

    public Stmt VisitWhileStmt(Stmt.While stmt)
    {
        if (!stmt.IsPreprocessed) return stmt;
        
        var output = new List<Stmt>();
        
        if (stmt.Condition.Accept(this) is not bool condition)
        {
            throw new Exception("While statement condition must be a constant boolean expression.");
        }
        
        while (condition)
        {
            if (stmt.Body.Accept(this) is { } result)
            {
                output.Add(result);
            }

            condition = (bool) stmt.Condition.Accept(this);
        }
        
        return new Stmt.Block
        {
            Statements = output
        };
    }

    public object VisitAssignExpr(Expr.Assign expr)
    {
        if (!Scope.TryGet(expr.Name.Lexeme, out var value))
        {
            return expr;
        }
        
        Scope.Assign(expr.Name.Lexeme, expr.Value.Accept(this));
        
        return null!;
    }

    public object VisitBinaryExpr(Expr.Binary expr)
    {
        var left = expr.Left.Accept(this);
        var right = expr.Right.Accept(this);

        return expr.Operator.Type switch
        {
            TokenType.PLUS          => (double)left + (double)right,
            TokenType.MINUS         => (double)left - (double)right,
            TokenType.STAR          => (double)left * (double)right,
            TokenType.SLASH         => (double)left / (double)right,
            TokenType.MODULO        => (double)left % (double)right,
            TokenType.GREATER       => (double)left > (double)right,
            TokenType.GREATER_EQUAL => (double)left >= (double)right,
            TokenType.LESS          => (double)left < (double)right,
            TokenType.LESS_EQUAL    => (double)left <= (double)right,
            TokenType.BANG_EQUAL    => !left.Equals(right),
            TokenType.EQUAL_EQUAL   => left.Equals(right),
            _ => throw new Exception($"Unknown binary operator {expr.Operator}")
        };
    }

    public object VisitCallExpr(Expr.Call expr)
    {
        // todo: check if the function is in the scope
        
        return expr;
    }

    public object VisitGroupingExpr(Expr.Grouping expr)
    {
        return expr;
    }

    public object VisitLiteralExpr(Expr.Literal expr)
    {
        if (expr.Value is not (string or bool or double or Array or IDictionary))
        {
            throw new Exception("Unsupported literal type.");
        }
        
        return expr.Value;
    }

    public object VisitObjectExpr(Expr.Object expr)
    {
        return expr;
    }

    public object VisitArrayExpr(Expr.Array expr)
    {
        return expr;
    }

    public object VisitLogicalExpr(Expr.Logical expr)
    {
        return expr;
    }

    public object VisitUnaryExpr(Expr.Unary expr)
    {
        return expr;
    }

    public object VisitVariableExpr(Expr.Variable expr)
    {
        if (!Scope.TryGet(expr.Name.Lexeme, out var value))
        {
            throw new Exception($"Undefined variable '{expr.Name.Lexeme}'.");
        }
        
        return value;
    }
}