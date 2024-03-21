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
         if (!File.Exists(filePath))
         {
             File.Create(filePath).Close();
         }
         File.AppendAllText(filePath, command + "\n");
     }
     
     private void AddInitCommand(string command)
     {
         var filePath = Path.Combine(OutDir, RootNamespace, "functions", Environment.Namespace, "_amethyst_init.mcfunction");
         if (!File.Exists(filePath))
         {
             File.Create(filePath).Close();
         }
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
        stmt.Accept(this);
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
        if (expr.Callee is Expr.Variable variable)
        {
            // var variableDefinition = Environment.GetVariable(variable.Name.Lexeme);
            // Todo: get function from environment. Depending on subject, set _out = _ret
            var functionPath = RootNamespace + ":" + Path.Combine(Environment.Namespace, variable.Name.Lexeme);
            AddCommand($"function {functionPath}");
            // AddCommand("scoreboard players operation _out amethyst = _ret amethyst");
            AddCommand("data modify storage amethyst:internal _out set from storage amethyst:internal _ret");
            // return variableDefinition.Subject;
            return Subject.Storage;
        }
        return null;
    }

    public object? VisitGroupingExpr(Expr.Grouping expr)
    {
        return Evaluate(expr.Expression);
    }

    public object? VisitLiteralExpr(Expr.Literal expr)
    {
        switch (expr.Value)
        {
            case string:
                AddCommand($"data modify storage amethyst:internal _out set value '{expr.Value}'");
                return Subject.Storage;
            case bool:
                AddCommand($"data modify storage amethyst:internal _out set value {expr.Value}");
                return Subject.Storage;
            case double:
                AddCommand($"scoreboard players set _out amethyst {expr.Value}");
                return Subject.Scoreboard;
        }

        return null;
    }

    public object? VisitObjectExpr(Expr.Object expr)
    {
        return null;
    }

    public object VisitArrayExpr(Expr.Array expr)
    {
        var name = Environment.GetUniqueName();
        Environment.AddVariable(name, Subject.Storage, out var variable);
        AddCommand($"data modify storage amethyst:internal {variable.Name} set value []");
        foreach (var exprValue in expr.Values)
        {
            if (Evaluate(exprValue) is Subject subject)
            {
                switch (subject)
                {
                    case Subject.Scoreboard:
                        AddCommand($"data modify storage amethyst:internal {variable.Name} append value 0");
                        AddCommand($"execute store result storage amethyst:internal {variable.Name}[-1] int 1.0 run scoreboard players get _out amethyst");
                        break;
                    case Subject.Storage:
                        AddCommand($"data modify storage amethyst:internal {variable.Name} append from storage amethyst:internal _out");
                        break;
                }
            }
        }
        AddCommand($"data modify storage amethyst:internal _out set from storage amethyst:internal {variable.Name}");
        AddCommand($"data remove storage amethyst:internal {variable.Name}");
        Environment.RemoveVariable(name);
        return Subject.Storage;
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
        if (!Environment.TryGetVariable(expr.Name.Lexeme, out var variable))
        {
            throw new SyntaxException($"Undefined variable {expr.Name.Lexeme}", expr.Name.Line);
        }
        switch (variable.Subject)
        {
            case Subject.Storage:
                AddCommand($"data modify storage amethyst:internal _out set from storage amethyst:internal {variable.Name}");
                return Subject.Storage;
            case Subject.Scoreboard:
                AddCommand($"scoreboard players operation _out amethyst = {variable.Name} amethyst");
                return Subject.Scoreboard;
        }
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
        // var variable = Environment.AddVariable(stmt.Name.Lexeme, Subject.Storage);
        // Todo: add function to environment
        var functionPath = RootNamespace + ":" + Path.Combine(Environment.Namespace, stmt.Name.Lexeme);
        
        if (stmt.Initializing)
        {
            Environment.InitializingFunctions.Add(functionPath);
            AddInitCommand($"function {functionPath}");
        }

        if (stmt.Ticking)
        {
            Environment.TickingFunctions.Add(functionPath);
        }
        
        var previous = Environment;
        Environment = new Environment(Environment, stmt.Name.Lexeme);
        
        AddCommand("scoreboard players reset _ret amethyst");
        AddCommand("data storage remove amethyst:internal _ret");

        try
        {
            foreach (var statement in stmt.Body)
            {
                Compile(statement);
            }
        }
        finally
        {
            Environment = previous;
        }
        
        return null;
    }

    public object? VisitIfStmt(Stmt.If stmt)
    {
        return null;
    }

    public object? VisitPrintStmt(Stmt.Print stmt)
    {
        if (Evaluate(stmt.Expr) is Subject subject)
        {
            switch (subject)
            {
                case Subject.Scoreboard:
                    AddCommand("tellraw @s {'score':{'name':'_out','objective':'amethyst'}}");
                    break;
                case Subject.Storage:
                    AddCommand("tellraw @s {'storage':'amethyst:internal','nbt':'_out'}");
                    break;
            }
        }
        return null;
    }

    public object? VisitCommentStmt(Stmt.Comment stmt)
    {
        AddCommand($"# {stmt.Value.Lexeme}");
        return null;
    }

    public object? VisitReturnStmt(Stmt.Return stmt)
    {
        if (Evaluate(stmt.Value) is Subject subject)
        {
            const string afterReturnFunctionName = "_amethyst_arf"; // after-return-function
            var afterReturnFunctionDirectory = Path.Combine(OutDir, RootNamespace, "functions", Environment.Namespace, Environment.CurrentFunction);
            Directory.CreateDirectory(afterReturnFunctionDirectory);
            
            var afterReturnFunctionPath = RootNamespace + ":" + Path.Combine(Environment.Namespace, Environment.CurrentFunction, afterReturnFunctionName);
            switch (subject)
            {
                case Subject.Scoreboard:
                    AddCommand("scoreboard players operation _ret amethyst = _out amethyst");
                    AddCommand($"execute unless score _ret amethyst matches -2147483648..2147483647 run function {afterReturnFunctionPath}");
                    break;
                case Subject.Storage:
                    AddCommand("data modify storage amethyst:internal _ret set from storage amethyst:internal _out");
                    AddCommand($"execute unless data storage amethyst:internal _ret run function {afterReturnFunctionPath}");
                    break;
            }
            
            Environment = new Environment(Environment, afterReturnFunctionName, Environment.CurrentFunction);
        }

        return null;
    }

    public object? VisitVarStmt(Stmt.Var stmt)
    {
        if (!Environment.AddVariable(stmt.Name.Lexeme, Subject.Storage, out var variable))
        {
            throw new SyntaxException($"Variable {stmt.Name.Lexeme} already defined", stmt.Name.Line);
        }

        if (Evaluate(stmt.Initializer) is Subject subject)
        {
            switch (subject)
            {
                case Subject.Scoreboard:
                    AddCommand($"scoreboard players operation {variable.Name} amethyst = _out amethyst");
                    break;
                case Subject.Storage:
                    AddCommand($"data modify storage amethyst:internal {variable.Name} set from storage amethyst:internal _out");
                    break;
            }
        }
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