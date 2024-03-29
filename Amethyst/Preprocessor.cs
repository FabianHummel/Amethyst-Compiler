namespace Amethyst;

public class Preprocessor : Stmt.IVisitor<Stmt>, Expr.IVisitor<object>
{
    private IList<Stmt> Statements { get; }
    
    public Preprocessor(IList<Stmt> stmts)
    {
        Statements = stmts;
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
        return stmt;
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

    public Stmt VisitIfStmt(Stmt.If stmt)
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

    public Stmt VisitPrintStmt(Stmt.Print stmt)
    {
        if (!stmt.IsPreprocessed) return stmt;

        Console.Out.WriteLine(stmt.Expr.Accept(this));
        
        return stmt;
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

    public Stmt VisitVarStmt(Stmt.Var stmt)
    {
        if (!stmt.IsPreprocessed) return stmt;
        
        // todo: add variable to the scope
        
        return stmt;
    }

    public Stmt VisitWhileStmt(Stmt.While stmt)
    {
        if (!stmt.IsPreprocessed) return stmt;
        
        if (stmt.Condition.Accept(this) is not bool condition)
        {
            throw new Exception("While statement condition must be a constant boolean expression.");
        }
        
        while (condition)
        {
            stmt.Body.Accept(this);

            condition = (bool) stmt.Condition.Accept(this);
        }
        
        return stmt;
    }

    public object VisitAssignExpr(Expr.Assign expr)
    {
        // todo: check if the variable is in the scope
        
        return expr;
    }

    public object VisitBinaryExpr(Expr.Binary expr)
    {
        var left = expr.Left.Accept(this);
        var right = expr.Right.Accept(this);

        return expr.Operator.Type switch
        {
            TokenType.PLUS => (double)left + (double)right,
            TokenType.MINUS => (double)left - (double)right,
            TokenType.STAR => (double)left * (double)right,
            TokenType.SLASH => (double)left / (double)right,
            TokenType.MODULO => (double)left % (double)right,
            TokenType.GREATER => (double)left > (double)right,
            TokenType.GREATER_EQUAL => (double)left >= (double)right,
            TokenType.LESS => (double)left < (double)right,
            TokenType.LESS_EQUAL => (double)left <= (double)right,
            TokenType.BANG_EQUAL => !left.Equals(right),
            TokenType.EQUAL_EQUAL => left.Equals(right),
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
        return expr;
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
        // todo: check if the variable is in the scope
        
        return expr;
    }
}