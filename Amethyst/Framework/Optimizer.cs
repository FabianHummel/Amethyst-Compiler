using Amethyst.Model;

namespace Amethyst.Framework;

public class Optimizer : Expr.IVisitor<Expr>, Stmt.IVisitor<Stmt>
{
    private IList<Stmt> Statements { get; }
    
    public Optimizer(IList<Stmt> statements)
    {
        Statements = statements;
    }
    
    public IList<Stmt> Optimize()
    {
        return OptimizeBlock(Statements);
    }

    private IList<Stmt> OptimizeBlock(IEnumerable<Stmt> statements)
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

    public Expr VisitAssignExpr(Expr.Assign expr)
    {
        return new Expr.Assign
        {
            Name = expr.Name,
            Value = expr.Value.Accept(this)
        };
    }

    public Expr VisitBinaryExpr(Expr.Binary expr)
    {
        var left = expr.Left.Accept(this);
        var right = expr.Right.Accept(this);
        
        if (left is Expr.Literal { Value: {} l } && right is Expr.Literal { Value: {} r })
        {
            return new Expr.Literal
            {
                Value = expr.Operator.Type switch
                {
                    TokenType.PLUS => DynamicArithmetics.Add(l, r),
                    TokenType.MINUS => DynamicArithmetics.Sub(l, r),
                    TokenType.STAR => DynamicArithmetics.Mul(l, r),
                    TokenType.SLASH => DynamicArithmetics.Div(l, r),
                    TokenType.MODULO => DynamicArithmetics.Mod(l, r),
                    TokenType.GREATER => DynamicArithmetics.Gt(l, r),
                    TokenType.GREATER_EQUAL => DynamicArithmetics.Ge(l, r),
                    TokenType.LESS => DynamicArithmetics.Lt(l, r),
                    TokenType.LESS_EQUAL => DynamicArithmetics.Le(l, r),
                    TokenType.BANG_EQUAL => DynamicArithmetics.Ne(l, r),
                    TokenType.EQUAL_EQUAL => DynamicArithmetics.Eq(l, r),
                    _ => throw new Exception($"Unknown binary operator {expr.Operator}")
                }
            };
        }

        return new Expr.Binary
        {
            Left = left,
            Operator = expr.Operator,
            Right = right
        };
    }

    public Expr VisitCallExpr(Expr.Call expr)
    {
        return expr;
    }

    public Expr VisitGroupingExpr(Expr.Grouping expr)
    {
        return expr.Expression.Accept(this);
    }

    public Expr VisitLiteralExpr(Expr.Literal expr)
    {
        return expr;
    }

    public Expr VisitObjectExpr(Expr.Object expr)
    {
        return expr;
    }

    public Expr VisitArrayExpr(Expr.Array expr)
    {
        var elements = new List<Expr>();
        
        foreach (var value in expr.Values)
        {
            elements.Add(value.Accept(this));
        }
        
        if (elements.All(e => e is Expr.Literal))
        {
            return new Expr.Literal
            {
                Value = elements.Select(e => ((Expr.Literal)e).Value).ToArray()
            };
        }
        
        return new Expr.Array
        {
            Values = elements
        };
    }

    public Expr VisitLogicalExpr(Expr.Logical expr)
    {
        var left = expr.Left.Accept(this);
        var right = expr.Right.Accept(this);
        
        if (left is Expr.Literal l && right is Expr.Literal r)
        {
            switch (expr.Operator.Type)
            {
                case TokenType.AND:
                {
                    if (l.Value is bool lb && r.Value is bool rb)
                    {
                        return new Expr.Literal
                        {
                            Value = lb && rb
                        };
                    }
                    break;
                }
                case TokenType.OR:
                {
                    if (l.Value is bool lb2 && r.Value is bool rb2)
                    {
                        return new Expr.Literal
                        {
                            Value = lb2 || rb2
                        };
                    }
                    break;
                }
            }
        }
        
        return new Expr.Logical
        {
            Left = left,
            Operator = expr.Operator,
            Right = right
        };
    }

    public Expr VisitUnaryExpr(Expr.Unary expr)
    {
        var right = expr.Right.Accept(this);
        
        if (right is Expr.Literal r)
        {
            switch (expr.Operator.Type)
            {
                case TokenType.MINUS:
                {
                    if (r.Value is double rd)
                    {
                        return new Expr.Literal
                        {
                            Value = -rd
                        };
                    }
                    break;
                }
                case TokenType.BANG:
                {
                    if (r.Value is bool rb)
                    {
                        return new Expr.Literal
                        {
                            Value = !rb
                        };
                    }
                    break;
                }
            }
        }
        
        return expr;
    }

    public Expr VisitVariableExpr(Expr.Variable expr)
    {
        return expr;
    }

    public Stmt VisitBlockStmt(Stmt.Block stmt)
    {
        return new Stmt.Block
        {
            Statements = OptimizeBlock(stmt.Statements)
        };
    }

    public Stmt VisitExpressionStmt(Stmt.Expression stmt)
    {
        return new Stmt.Expression
        {
            Expr = stmt.Expr.Accept(this)
        };
    }

    public Stmt VisitNamespaceStmt(Stmt.Namespace stmt)
    {
        return new Stmt.Namespace
        {
            Name = stmt.Name,
            Body = OptimizeBlock(stmt.Body),
            IsPreprocessed = stmt.IsPreprocessed
        };
    }

    public Stmt VisitFunctionStmt(Stmt.Function stmt)
    {
        return new Stmt.Function
        {
            Ticking = stmt.Ticking,
            Initializing = stmt.Initializing,
            Name = stmt.Name,
            Params = stmt.Params,
            Body = OptimizeBlock(stmt.Body),
            IsPreprocessed = stmt.IsPreprocessed
        };
    }

    public Stmt VisitIfStmt(Stmt.If stmt)
    {
        return new Stmt.If
        {
            Condition = stmt.Condition.Accept(this),
            ThenBranch = stmt.ThenBranch.Accept(this),
            ElseBranch = stmt.ElseBranch?.Accept(this),
            IsPreprocessed = stmt.IsPreprocessed
        };
    }

    public Stmt VisitPrintStmt(Stmt.Print stmt)
    {
        return new Stmt.Print
        {
            Expr = stmt.Expr.Accept(this),
            IsPreprocessed = stmt.IsPreprocessed
        };
    }

    public Stmt VisitCommentStmt(Stmt.Comment stmt)
    {
        return stmt;
    }

    public Stmt VisitReturnStmt(Stmt.Return stmt)
    {
        return new Stmt.Return
        {
            Value = stmt.Value?.Accept(this),
            IsPreprocessed = stmt.IsPreprocessed
        };
    }

    public Stmt VisitBreakStmt(Stmt.Break stmt)
    {
        return stmt;
    }

    public Stmt VisitContinueStmt(Stmt.Continue stmt)
    {
        return stmt;
    }

    public Stmt VisitVarStmt(Stmt.Var stmt)
    {
        return new Stmt.Var
        {
            Name = stmt.Name,
            Initializer = stmt.Initializer.Accept(this),
            IsPreprocessed = stmt.IsPreprocessed
        };
    }

    public Stmt VisitWhileStmt(Stmt.While stmt)
    {
        return new Stmt.While
        {
            Condition = stmt.Condition.Accept(this),
            Body = stmt.Body.Accept(this),
            IsPreprocessed = stmt.IsPreprocessed
        };
    }
}