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
        OutDir = Path.Combine(outDir, "data");
        Environment = new Environment("_init");
        RootNamespace = rootNamespace;
    }
    
     private void AddCommand(string command)
     {
         if (Environment.Enclosing == null)
         {
             Environment.InitializingFunctions.Add(RootNamespace + ":" + Path.Combine(Environment.Namespace, "_init"));
         }
         
         var filePath = Path.Combine(OutDir, RootNamespace, "functions", Environment.Namespace, Environment.CurrentFunction + ".mcfunction");
         if (!File.Exists(filePath))
         {
             File.Create(filePath).Close();
         }
         File.AppendAllText(filePath, command + "\n");
     }
     
     private void AddInitCommand(string command)
     {
         Environment.InitializingFunctions.Add(RootNamespace + ":" + Path.Combine(Environment.Namespace, "_init"));
         
         var filePath = Path.Combine(OutDir, RootNamespace, "functions", Environment.Namespace, "_init.mcfunction");
         if (!File.Exists(filePath))
         {
             File.Create(filePath).Close();
         }
         File.AppendAllText(filePath, command + "\n");
     }

    private void CompileBlock(IEnumerable<Stmt> statements, string currentFunction = "_init", string scope = "")
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
        // Todo: implement binary expressions
        AddCommand("scoreboard players set _out amethyst 0");
        return Subject.Scoreboard;
    }

    public object? VisitCallExpr(Expr.Call expr)
    {
        if (expr.Callee is Expr.Variable variable)
        {
            // var variableDefinition = Environment.GetVariable(variable.Name.Lexeme);
            // Todo: get function from environment. Depending on subject, set _out = _ret
            var functionPath = RootNamespace + ":" + Path.Combine(Environment.Namespace, variable.Name.Lexeme);
            AddCommand($"function {functionPath}");
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
                AddCommand($"data modify storage amethyst:internal _out set value \"{expr.Value}\"");
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
        }

        if (stmt.Ticking)
        {
            Environment.TickingFunctions.Add(functionPath);
        }
        
        var previous = Environment;
        Environment = new Environment(Environment, stmt.Name.Lexeme);
        CompileBlock(stmt.Body, stmt.Name.Lexeme);
        Environment = previous;
        return null;
    }

    public object? VisitIfStmt(Stmt.If stmt)
    {
        var functionDirectory = Path.Combine(OutDir, RootNamespace, "functions", Environment.Namespace, Environment.CurrentFunction);
        Directory.CreateDirectory(functionDirectory);

        var depth = Environment.IfCounter++;
        string ifBranchName = "_if" + depth;
        const string thenBranchName = "_then";
        const string elseBranchName = "_else";
        
        var ifFunctionPath = RootNamespace + ":" + Path.Combine(Environment.Namespace, Environment.CurrentFunction, ifBranchName);
        AddCommand($"execute store result storage amethyst:internal _out store success score _out amethyst run function {ifFunctionPath}");
        AddCommand("execute if score _out amethyst matches 1 run return data get storage amethyst:internal _out");
        var controlFlowEnvironment = Environment;
        Environment = new Environment(Environment, ifBranchName, Environment.CurrentFunction);
        {
            var controlFlowDirectory = Path.Combine(OutDir, RootNamespace, "functions", Environment.Namespace, Environment.CurrentFunction);
            Directory.CreateDirectory(controlFlowDirectory);
            var ifStmtEnvironment = Environment;
            {
                if (Evaluate(stmt.Condition) is Subject.Scoreboard)
                {
                    AddCommand($"scoreboard players operation _cond{depth} amethyst = _out amethyst");
                }
                AddCommand("scoreboard players reset _out amethyst");
                
                var thenEnvironment = Environment;
                Environment = new Environment(Environment, thenBranchName, Environment.CurrentFunction);
                var thenFunctionPath = RootNamespace + ":" + Path.Combine(Environment.Namespace, Environment.CurrentFunction);
                Compile(stmt.ThenBranch);
                Environment = thenEnvironment;
                
                AddCommand($"execute if score _cond{depth} amethyst matches 1 store result storage amethyst:internal _out store success score _out amethyst run function {thenFunctionPath}");
                AddCommand("execute if score _out amethyst matches 1 run return data get storage amethyst:internal _out");
                AddCommand("execute if score _out amethyst matches 0 if score _brk amethyst matches 1 run return fail");
                
                var elseEnvironment = Environment;
                Environment = new Environment(Environment, elseBranchName, Environment.CurrentFunction);
                var elseFunctionPath = RootNamespace + ":" + Path.Combine(Environment.Namespace, Environment.CurrentFunction);
                if (stmt.ElseBranch != null)
                {
                    Compile(stmt.ElseBranch);
                }
                Environment = elseEnvironment;

                if (stmt.ElseBranch != null)
                {
                    AddCommand($"execute if score _cond{depth} amethyst matches 1 store result storage amethyst:internal _out store success score _out amethyst run function {elseFunctionPath}");
                    AddCommand("execute if score _out amethyst matches 1 run return data get storage amethyst:internal _out");
                    AddCommand("execute if score _out amethyst matches 0 if score _brk amethyst matches 1 run return fail");
                }
            }
            Environment = ifStmtEnvironment;
        }
        Environment = controlFlowEnvironment;
        return null;
    }

    public object? VisitPrintStmt(Stmt.Print stmt)
    {
        if (Evaluate(stmt.Expr) is Subject subject)
        {
            switch (subject)
            {
                case Subject.Scoreboard:
                    AddCommand("tellraw @s {\"score\":{\"name\":\"_out\",\"objective\":\"amethyst\"}}");
                    break;
                case Subject.Storage:
                    AddCommand("tellraw @s {\"storage\":\"amethyst:internal\",\"nbt\":\"_out\"}");
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
        if (stmt.Value == null)
        {
            AddCommand("return 1");
        }
        
        else if (Evaluate(stmt.Value) is Subject subject)
        {
            switch (subject)
            {
                case Subject.Scoreboard:
                    AddCommand("return run scoreboard players get _out amethyst");
                    break;
                case Subject.Storage:
                    AddCommand("return run data get storage amethyst:internal _out");
                    break;
            }
        }

        return null;
    }

    public object? VisitBreakStmt(Stmt.Break stmt)
    {
        AddCommand("scoreboard players set _brk amethyst 1");
        AddCommand("return fail");
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