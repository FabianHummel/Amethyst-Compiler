using System.Diagnostics.CodeAnalysis;

namespace Amethyst;

public class Preprocessor : Stmt.IVisitor<Stmt?>, Expr.IVisitor<Expr?>
{
    private class Environment
    {
        public Environment? Enclosing { get; init; }
        public Dictionary<string, object?> Values { get; } = new();
        public Dictionary<string, Stmt.Function> Functions { get; } = new();
        public IList<Stmt> CurrentStmts { get; set; } = new List<Stmt>();
        
        public bool IsVariableDefined(string name)
        {
            return Values.ContainsKey(name) || Enclosing?.IsVariableDefined(name) == true;
        }
        
        public bool IsFunctionDefined(string name)
        {
            return Functions.ContainsKey(name) || Enclosing?.IsFunctionDefined(name) == true;
        }
        
        public bool AssignVariable(string name, object value)
        {
            if (Values.ContainsKey(name))
            {
                Values[name] = value;
                return true;
            }

            return Enclosing?.AssignVariable(name, value) == true;
        }
        
        public bool TryGetVariable(string name, out object? value)
        {
            if (Values.TryGetValue(name, out value))
            {
                return true;
            }

            return Enclosing?.TryGetVariable(name, out value) == true;
        }
        
        public bool TryGetFunction(string name, [MaybeNullWhen(false)] out Stmt.Function value)
        {
            if (Functions.TryGetValue(name, out value))
            {
                return true;
            }

            return Enclosing?.TryGetFunction(name, out value) == true;
        }
    }
    
    private IList<Stmt> Statements { get; }
    private Environment Scope { get; set; }
    
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
        var previous = Scope;
        Scope = new Environment
        {
            Enclosing = previous
        };
        
        var stmts = Scope.CurrentStmts = new List<Stmt>();
        
        foreach (var statement in statements)
        {
            if (statement.Accept(this) is { } stmt)
            {
                stmts.Add(stmt);
            }
        }
        
        Scope = previous;
        
        return stmts;
    }

    public Stmt VisitBlockStmt(Stmt.Block stmt)
    {
        return new Stmt.Block
        {
            Statements = PreprocessBlock(stmt.Statements)
        };
    }

    public Stmt? VisitExpressionStmt(Stmt.Expression stmt)
    {
        if (stmt.Expr.Accept(this) is { } expr)
        {
            return new Stmt.Expression
            {
                Expr = expr
            };
        }
        
        return null;
    }

    public Stmt? VisitNamespaceStmt(Stmt.Namespace stmt)
    {
        if (!stmt.IsPreprocessed)
        {
            return new Stmt.Namespace
            {
                Name = stmt.Name,
                Body = PreprocessBlock(stmt.Body),
                IsPreprocessed = true
            };
        }
        
        // todo: add namespace to the scope

        return null;
    }

    public Stmt? VisitFunctionStmt(Stmt.Function stmt)
    {
        if (!stmt.IsPreprocessed)
        {
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

        foreach (var param in stmt.Params)
        {
            if (Scope.IsVariableDefined(param.Lexeme))
            {
                throw new Exception($"Variable '{param.Lexeme}' is already defined.");
            }
            
            Scope.Values[param.Lexeme] = null;
        }

        if (Scope.IsFunctionDefined(stmt.Name.Lexeme))
        {
            throw new Exception($"Function '{stmt.Name.Lexeme}' is already defined.");
        }

        Scope.Functions[stmt.Name.Lexeme] = stmt;

        return null;
    }

    public Stmt? VisitIfStmt(Stmt.If stmt)
    {
        if (!stmt.IsPreprocessed)
        {
            return new Stmt.If
            {
                Condition = stmt.Condition.Accept(this) ?? throw new Exception("If statement must have a condition."),
                ThenBranch = stmt.ThenBranch.Accept(this) ?? throw new Exception("If statement must have a body."),
                ElseBranch = stmt.ElseBranch?.Accept(this),
                IsPreprocessed = true
            };
        }
        
        if (stmt.Condition.Accept(this) is not Expr.Literal { Value: bool condition })
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
            Expr = stmt.Expr.Accept(this) ?? throw new Exception("Print statement must have an expression."),
            IsPreprocessed = true
        };

        Console.Out.Write(stmt.Expr.Accept(this));

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
        
        if (stmt.Initializer?.Accept(this) is Expr.Literal { Value: { } value })
        {
            Scope.Values[stmt.Name.Lexeme] = value;
        }

        return null;
    }

    public Stmt VisitWhileStmt(Stmt.While stmt)
    {
        if (!stmt.IsPreprocessed) return stmt;
        
        var output = new List<Stmt>();
        
        if (stmt.Condition.Accept(this) is not Expr.Literal { Value: bool condition })
        {
            throw new Exception("While statement condition must be a constant boolean expression.");
        }
        
        while (condition)
        {
            if (stmt.Body.Accept(this) is { } result)
            {
                output.Add(result);
            }

            if (stmt.Condition.Accept(this) is not Expr.Literal { Value: bool newCondition })
            {
                throw new Exception("While statement condition must be a constant boolean expression.");
            }
            
            condition = newCondition;
        }
        
        return new Stmt.Block
        {
            Statements = output
        };
    }

    public Expr? VisitAssignExpr(Expr.Assign expr)
    {
        if (!Scope.IsVariableDefined(expr.Name.Lexeme))
        {
            return expr;
        }
        
        if (expr.Value.Accept(this) is Expr.Literal { Value: { } value })
        {
            Scope.AssignVariable(expr.Name.Lexeme, value);
        }

        return null;
    }

    public Expr VisitBinaryExpr(Expr.Binary expr)
    {
        if (expr.Left.Accept(this) is not Expr.Literal { Value: { } left })
        {
            return expr;
        }
        
        if (expr.Right.Accept(this) is not Expr.Literal { Value: { } right })
        {
            return expr;
        }

        return new Expr.Literal
        {
            Value = expr.Operator.Type switch
            {
                TokenType.PLUS          => DynamicArithmetics.Add(left, right),
                TokenType.MINUS         => DynamicArithmetics.Sub(left, right),
                TokenType.STAR          => DynamicArithmetics.Mul(left, right),
                TokenType.SLASH         => DynamicArithmetics.Div(left, right),
                TokenType.MODULO        => DynamicArithmetics.Mod(left, right),
                TokenType.GREATER       => DynamicArithmetics.Gt(left, right),
                TokenType.GREATER_EQUAL => DynamicArithmetics.Ge(left, right),
                TokenType.LESS          => DynamicArithmetics.Lt(left, right),
                TokenType.LESS_EQUAL    => DynamicArithmetics.Le(left, right),
                TokenType.BANG_EQUAL    => DynamicArithmetics.Ne(left, right),
                TokenType.EQUAL_EQUAL   => DynamicArithmetics.Eq(left, right),
                _ => throw new Exception($"Unknown binary operator {expr.Operator}")
            }
        };
    }

    public Expr? VisitCallExpr(Expr.Call expr)
    {
        if (expr.Callee is Expr.Variable variable && Scope.TryGetFunction(variable.Name.Lexeme, out var function))
        {
            try
            {
                foreach (var (param, arg) in function.Params.Zip(expr.Arguments))
                {
                    if (arg.Accept(this) is Expr.Literal { Value: { } value })
                    {
                        Scope.Values[param.Lexeme] = value;
                    }
                }
                foreach (var stmt in PreprocessBlock(function.Body))
                {
                    Scope.CurrentStmts.Add(stmt);
                }
            }
            catch (Return e)
            {
                return e.Value;
            }
            finally
            {
                foreach (var param in function.Params)
                {
                    Scope.Values.Remove(param.Lexeme);
                }
            }
            
            return null;
        }
        
        return expr;
    }

    public Expr? VisitGroupingExpr(Expr.Grouping expr)
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
        return expr;
    }

    public Expr VisitLogicalExpr(Expr.Logical expr)
    {
        if (expr.Left.Accept(this) is not Expr.Literal { Value: { } left })
        {
            return expr;
        }
        
        if (expr.Right.Accept(this) is not Expr.Literal { Value: { } right })
        {
            return expr;
        }
        
        return new Expr.Literal
        {
            Value = expr.Operator.Type switch
            {
                TokenType.AND => DynamicArithmetics.And(left, right),
                TokenType.OR  => DynamicArithmetics.Or(left, right),
                _ => throw new Exception($"Unknown logical operator {expr.Operator}")
            }
        };
    }

    public Expr VisitUnaryExpr(Expr.Unary expr)
    {
        return expr;
    }

    public Expr VisitVariableExpr(Expr.Variable expr)
    {
        if (!Scope.TryGetVariable(expr.Name.Lexeme, out var value))
        {
            throw new Exception($"Undefined variable '{expr.Name.Lexeme}'.");
        }
        
        return new Expr.Literal
        {
            Value = value ?? throw new Exception($"Variable '{expr.Name.Lexeme}' is not initialized when used.")
        };
    }
}