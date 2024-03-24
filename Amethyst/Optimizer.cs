namespace Amethyst;

public class Optimizer : Expr.IVisitor<Expr>, Stmt.IVisitor<Stmt>
{
    private IList<Stmt> Statements { get; }
    
    public Optimizer(IList<Stmt> statements)
    {
        Statements = statements;
    }
    
    public IList<Stmt> Optimize()
    {
        var statements = new List<Stmt>();
        
        foreach (var statement in Statements)
        {
            if (statement.Accept(this) is { } stmt)
            {
                statements.Add(stmt);
            }
        }
        
        return statements;
    }

    public Expr VisitAssignExpr(Expr.Assign expr)
    {
        return expr;
    }

    public Expr VisitBinaryExpr(Expr.Binary expr)
    {
        var left = expr.Left.Accept(this);
        var right = expr.Right.Accept(this);
        
        if (left is Expr.Literal l && right is Expr.Literal r)
        {
            switch (expr.Operator.Type)
            {
                case TokenType.PLUS:
                {
                    if (l.Value is double ld && r.Value is double rd)
                    {
                        return new Expr.Literal
                        {
                            Value = ld + rd
                        };
                    }

                    if (l.Value is string ls2 && r.Value is double rd2)
                    {
                        return new Expr.Literal
                        {
                            Value = ls2 + rd2
                        };
                    }

                    return new Expr.Literal
                    {
                        Value = string.Concat(l.Value, r.Value)
                    };
                }
                case TokenType.MINUS:
                {
                    if (l.Value is double ld2 && r.Value is double rd2)
                    {
                        return new Expr.Literal
                        {
                            Value = ld2 - rd2
                        };
                    }
                    break;
                }
                case TokenType.STAR:
                {
                    if (l.Value is double ld3 && r.Value is double rd3)
                    {
                        return new Expr.Literal
                        {
                            Value = ld3 * rd3
                        };
                    }
                    break;
                }
                case TokenType.SLASH:
                {
                    if (l.Value is double ld4 && r.Value is double rd4)
                    {
                        return new Expr.Literal
                        {
                            Value = ld4 / rd4
                        };
                    }
                    break;
                }
                case TokenType.MODULO:
                {
                    if (l.Value is double ld5 && r.Value is double rd5)
                    {
                        return new Expr.Literal
                        {
                            Value = ld5 % rd5
                        };
                    }
                    break;
                }
                case TokenType.GREATER:
                {
                    if (l.Value is double ld6 && r.Value is double rd6)
                    {
                        return new Expr.Literal
                        {
                            Value = ld6 > rd6
                        };
                    }
                    break;
                }
                case TokenType.GREATER_EQUAL:
                {
                    if (l.Value is double ld7 && r.Value is double rd7)
                    {
                        return new Expr.Literal
                        {
                            Value = ld7 >= rd7
                        };
                    }
                    break;
                }
                case TokenType.LESS:
                {
                    if (l.Value is double ld8 && r.Value is double rd8)
                    {
                        return new Expr.Literal
                        {
                            Value = ld8 < rd8
                        };
                    }
                    break;
                }
                case TokenType.LESS_EQUAL:
                {
                    if (l.Value is double ld9 && r.Value is double rd9)
                    {
                        return new Expr.Literal
                        {
                            Value = ld9 <= rd9
                        };
                    }
                    break;
                }
                case TokenType.EQUAL_EQUAL:
                {
                    return new Expr.Literal
                    {
                        Value = l.Value == r.Value
                    };
                }
                case TokenType.BANG_EQUAL:
                {
                    return new Expr.Literal
                    {
                        Value = l.Value != r.Value
                    };
                }
            }
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
        return stmt;
    }

    public Stmt VisitExpressionStmt(Stmt.Expression stmt)
    {
        return stmt;
    }

    public Stmt VisitNamespaceStmt(Stmt.Namespace stmt)
    {
        return stmt;
    }

    public Stmt VisitFunctionStmt(Stmt.Function stmt)
    {
        return stmt;
    }

    public Stmt VisitIfStmt(Stmt.If stmt)
    {
        return stmt;
    }

    public Stmt VisitPrintStmt(Stmt.Print stmt)
    {
        return new Stmt.Print
        {
            Expr = stmt.Expr.Accept(this)
        };
    }

    public Stmt VisitCommentStmt(Stmt.Comment stmt)
    {
        return stmt;
    }

    public Stmt VisitReturnStmt(Stmt.Return stmt)
    {
        return stmt;
    }

    public Stmt VisitBreakStmt(Stmt.Break stmt)
    {
        return stmt;
    }

    public Stmt VisitVarStmt(Stmt.Var stmt)
    {
        return stmt;
    }

    public Stmt VisitWhileStmt(Stmt.While stmt)
    {
        return stmt;
    }
}