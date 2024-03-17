namespace Amethyst;

public class Compiler : Expr.IVisitor<object?>, Stmt.IVisitor<object?>
{
    private IList<Stmt> Statements { get; }
    private string OutDir { get; set; }
    private Environment Environment { get; set; }
    private string RootNamespace { get; init; }
    
    public Compiler(IList<Stmt> statements, string rootNamespace, string outDir)
    {
        Statements = statements;
        OutDir = outDir;
        Environment = new Environment("_amethyst_init");
        RootNamespace = rootNamespace;
    }
    
     private void AddCommand(string command)
     {
         var filePath = Path.Combine(OutDir, RootNamespace, "functions", Environment.Namespace, Environment.CurrentFunction + ".mcfunction");
         File.Create(filePath).Close();
         File.AppendAllText(filePath, command + "\n");
     }
     
     private void AddInitCommand(string command)
     {
         var filePath = Path.Combine(OutDir, RootNamespace, "functions", Environment.Namespace, "_amethyst_init.mcfunction");
         File.Create(filePath).Close();
         File.AppendAllText(filePath, command + "\n");
     }

    private void CompileBlock(IEnumerable<Stmt> statements, string currentFunction = "_amethyst_init", string scope = "")
    {
        var previous = Environment;
        Environment = new Environment(Environment, currentFunction, scope);
        foreach (var statement in statements)
        {
            Compile(statement);
        }
        Environment = previous;
    }

    private void Compile(Stmt stmt)
    {
        stmt.Accept(this); // don't use return value, as it's always null
    }
    
    /// <summary>
    /// Jumps to the Expression's associated visit* function
    /// </summary>
    /// <param name="expr"></param>
    /// <returns></returns>
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

    public object? VisitBlockStmt(Stmt.Block stmt)
    {
        CompileBlock(stmt.Statements, Environment.CurrentFunction);
        return null;
    }

    public object? VisitExpressionStmt(Stmt.Expression stmt)
    {
        Evaluate(stmt.Expr);
        return null;
    }

    public object? VisitNamespaceStmt(Stmt.Namespace stmt)
    {
        var filePath = Path.Combine(OutDir, RootNamespace, "functions", Environment.Namespace, stmt.Name.Lexeme);
        Directory.CreateDirectory(filePath);
        
        CompileBlock(stmt.Body, scope: stmt.Name.Lexeme);
        
        return null;
    }

    public object? VisitFunctionStmt(Stmt.Function stmt)
    {
        var functionPath = RootNamespace + ":" + Path.Combine(Environment.Namespace, stmt.Name.Lexeme);
        
        if (stmt.Initializing)
        {
            Environment.InitializingFunctions.Add(functionPath);
        }

        if (stmt.Ticking)
        {
            Environment.TickingFunctions.Add(functionPath);
        }
        
        var filePath = Path.Combine(OutDir, RootNamespace, "functions", Environment.Namespace, stmt.Name.Lexeme + ".mcfunction");
        File.Create(filePath).Close();
        
        CompileBlock(stmt.Body, Environment.CurrentFunction);
        
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
        Project.CopyAmethystInternalModule(OutDir);
        
        foreach (var statement in Statements)
        {
            Compile(statement);
        }
        
        Project.CreateFunctionTags(OutDir, Environment);
    }
}