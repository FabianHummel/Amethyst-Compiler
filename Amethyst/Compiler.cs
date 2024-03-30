using System.Diagnostics.CodeAnalysis;

namespace Amethyst;

public class Compiler : Expr.IVisitor<object?>, Stmt.IVisitor<object?>
{
    private class Environment
    {
        public class Variable
        {
            public string Name { get; }
            public Subject Subject { get; set; }
            
            public Variable(string name, Subject subject)
            {
                Name = name;
                Subject = subject;
            }
        }
        
        private string Scope { get; }
        public string CurrentFunction { get; }
        public Environment? Enclosing { get; }
        public ISet<string> TickingFunctions { get; } = new HashSet<string>();
        public ISet<string> InitializingFunctions { get; } = new HashSet<string>();
        public int IfCounter { get; set; }
        public int LoopCounter { get; set; }
        public int BinaryCounter { get; set; }
        
        private IDictionary<string, Variable> Values { get; } = new Dictionary<string, Variable>();
        private ISet<string> VariableNames { get; } = new HashSet<string>();

        public string Namespace => Path.Combine(Enclosing?.Namespace ?? "", Scope);

        public Environment(string currentFunction)
        {
            Enclosing = null;
            Scope = "";
            CurrentFunction = currentFunction;
        }

        public Environment(Environment enclosing, string currentFunction, string scope = "")
        {
            Enclosing = enclosing;
            Scope = scope;
            CurrentFunction = currentFunction;
        }

        private bool IsVariableDefined(string name)
        {
            return VariableNames.Contains(name) || Enclosing?.IsVariableDefined(name) == true;
        }
        
        public string GetUniqueName()
        {
            string name;
            do
            {
                name = $"_v{BinaryCounter++}";
            } while (IsVariableDefined(name));
            return name;
        }

        public bool AddVariable(string targetName, Subject subject, out Variable variable)
        {
            var name = GetUniqueName();
            VariableNames.Add(name);
            variable = new Variable(name, subject);
            return Values.TryAdd(targetName, variable);
        }
        
        public bool TryGetVariable(string targetName, [NotNullWhen(true)] out Variable? variable)
        {
            if (Values.TryGetValue(targetName, out variable))
            {
                return true;
            }
            
            if (Enclosing?.TryGetVariable(targetName, out variable) == true)
            {
                return true;
            }

            return false;
        }
        
        public bool RemoveVariable(string targetName)
        {
            if (Values.Remove(targetName))
            {
                return true;
            }
            
            Enclosing?.RemoveVariable(targetName);
            return false;
        }
    }
    
    private IList<Stmt> Statements { get; }
    private string OutDir { get; set; }
    private Environment Scope { get; set; }
    private string RootNamespace { get; init; }
    
    public Compiler(IList<Stmt> statements, string rootNamespace, string outDir)
    {
        Statements = statements;
        OutDir = Path.Combine(outDir, "data");
        Scope = new Environment("_init");
        RootNamespace = rootNamespace;
    }
    
    public void Compile()
    {
        Project.CopyAmethystInternalModule(OutDir);
        
        foreach (var statement in Statements)
        {
            Compile(statement);
        }
        
        Project.CreateFunctionTags(OutDir, Scope.InitializingFunctions, Scope.TickingFunctions);
    }
    
     private void AddCommand(string command)
     {
         if (Scope.Enclosing == null)
         {
             Scope.InitializingFunctions.Add(RootNamespace + ":" + Path.Combine(Scope.Namespace, "_init"));
         }
         
         var filePath = Path.Combine(OutDir, RootNamespace, "functions", Scope.Namespace, Scope.CurrentFunction + ".mcfunction");
         if (!File.Exists(filePath))
         {
             File.Create(filePath).Close();
         }
         File.AppendAllText(filePath, command + "\n");
     }
     
     private void AddInitCommand(string command)
     {
         Scope.InitializingFunctions.Add(RootNamespace + ":" + Path.Combine(Scope.Namespace, "_init"));
         
         var filePath = Path.Combine(OutDir, RootNamespace, "functions", Scope.Namespace, "_init.mcfunction");
         if (!File.Exists(filePath))
         {
             File.Create(filePath).Close();
         }
         File.AppendAllText(filePath, command + "\n");
     }

    private void CompileBlock(IEnumerable<Stmt> statements, string currentFunction = "_init", string scope = "")
    {
        var previous = Scope;
        Scope = new Environment(Scope, currentFunction, scope);
        foreach (var statement in statements)
        {
            Compile(statement);
        }
        Scope = previous;
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
        if (!Scope.TryGetVariable(expr.Name.Lexeme, out var variable))
        {
            throw new SyntaxException($"Undefined variable {expr.Name.Lexeme}", expr.Name.Line);
        }

        if (Evaluate(expr.Value) is not Subject subject)
        {
            throw new SyntaxException("Invalid assignment", expr.Name.Line);
        }
        
        variable.Subject = subject;
        switch (subject)
        {
            case Subject.Scoreboard:
                AddCommand($"scoreboard players operation {variable.Name} amethyst = _out amethyst");
                break;
            case Subject.Storage:
                AddCommand($"data modify storage amethyst:internal {variable.Name} set from storage amethyst:internal _out");
                break;
        }
        
        return null;
    }

    public object? VisitBinaryExpr(Expr.Binary expr)
    {
        var depth = Scope.BinaryCounter++;
        
        var right = Evaluate(expr.Right);

        if (right is Subject.Scoreboard)
        {
            AddCommand($"scoreboard players operation _tmp{depth} amethyst = _out amethyst");
            
            if (Evaluate(expr.Left) is not Subject subject)
            {
                throw new SyntaxException("Invalid binary operation", expr.Operator.Line);
            }
            
            switch (expr.Operator.Type, subject)
            {
                case (TokenType.MINUS, Subject.Scoreboard):
                    AddCommand($"scoreboard players operation _out amethyst -= _tmp{depth} amethyst");
                    return Subject.Scoreboard;
                case (TokenType.PLUS, Subject.Scoreboard):
                    AddCommand($"scoreboard players operation _out amethyst += _tmp{depth} amethyst");
                    return Subject.Scoreboard;
                case (TokenType.SLASH, Subject.Scoreboard):
                    AddCommand($"scoreboard players operation _out amethyst /= _tmp{depth} amethyst");
                    AddCommand($"scoreboard players operation _out amethyst *= 100 amethyst_const");
                    return Subject.Scoreboard;
                case (TokenType.STAR, Subject.Scoreboard):
                    AddCommand($"scoreboard players operation _out amethyst *= _tmp{depth} amethyst");
                    AddCommand($"scoreboard players operation _out amethyst /= 100 amethyst_const");
                    return Subject.Scoreboard;
                case (TokenType.MODULO, Subject.Scoreboard):
                    AddCommand($"scoreboard players operation _out amethyst %= _tmp{depth} amethyst");
                    return Subject.Scoreboard;
                case (TokenType.BANG_EQUAL, Subject.Scoreboard):
                    AddCommand($"execute store success score _out amethyst unless score _out amethyst = _tmp{depth} amethyst");
                    return Subject.Scoreboard;
                case (TokenType.EQUAL_EQUAL, Subject.Scoreboard):
                    AddCommand($"execute store success score _out amethyst if score _out amethyst = _tmp{depth} amethyst");
                    return Subject.Scoreboard;
                case (TokenType.GREATER, Subject.Scoreboard):
                    AddCommand($"execute store success score _out amethyst if score _out amethyst > _tmp{depth} amethyst");
                    return Subject.Scoreboard;
                case (TokenType.GREATER_EQUAL, Subject.Scoreboard):
                    AddCommand($"execute store success score _out amethyst if score _out amethyst >= _tmp{depth} amethyst");
                    return Subject.Scoreboard;
                case (TokenType.LESS, Subject.Scoreboard):
                    AddCommand($"execute store success score _out amethyst if score _out amethyst < _tmp{depth} amethyst");
                    return Subject.Scoreboard;
                case (TokenType.LESS_EQUAL, Subject.Scoreboard):
                    AddCommand($"execute store success score _out amethyst if score _out amethyst <= _tmp{depth} amethyst");
                    return Subject.Scoreboard;
                default:
                    throw new SyntaxException("Invalid binary operator", expr.Operator.Line);
            }
        }
        
        return null;
    }

    public object? VisitCallExpr(Expr.Call expr)
    {
        if (expr.Callee is Expr.Variable variable)
        {
            // var variableDefinition = Environment.GetVariable(variable.Name.Lexeme);
            // Todo: get function from environment. Depending on subject, set _out = _ret
            var functionPath = RootNamespace + ":" + Path.Combine(Scope.Namespace, variable.Name.Lexeme);
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
            case string value:
                AddCommand($"data modify storage amethyst:internal _out set value \"{value}\"");
                return Subject.Storage;
            case bool value:
                AddCommand($"scoreboard players set _out amethyst {(value ? 1 : 0)}");
                return Subject.Scoreboard;
            case double value:
                AddCommand($"scoreboard players set _out amethyst {(int)(value * 100)}");
                return Subject.Scoreboard;
            case object[] value:
                AddCommand($"data modify storage amethyst:internal _out set value [{string.Join(',', value)}]");
                return Subject.Storage;
        }

        return null;
    }

    public object? VisitObjectExpr(Expr.Object expr)
    {
        return null;
    }

    public object VisitArrayExpr(Expr.Array expr)
    {
        var name = Scope.GetUniqueName();
        Scope.AddVariable(name, Subject.Storage, out var variable);
        
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
        Scope.RemoveVariable(name);
        return Subject.Storage;
    }

    public object? VisitLogicalExpr(Expr.Logical expr)
    {
        return null;
    }

    public object VisitUnaryExpr(Expr.Unary expr)
    {
        if (Evaluate(expr.Right) is Subject subject)
        {
            switch (expr.Operator.Type, subject)
            {
                case (TokenType.MINUS, Subject.Scoreboard):
                    AddCommand("scoreboard players operation _out amethyst *= -1 amethyst_const");
                    return Subject.Scoreboard;
                case (TokenType.MINUS, Subject.Storage):
                    throw new SyntaxException("Can't apply unary minus to storage", expr.Operator.Line);
                case (TokenType.BANG, Subject.Scoreboard):
                    AddCommand("execute store success score _out amethyst if score _out amethyst matches 0");
                    return Subject.Scoreboard;
                case (TokenType.BANG, Subject.Storage):
                    throw new SyntaxException("Can't apply logical not to storage", expr.Operator.Line);
            }
        }
        
        throw new SyntaxException("Invalid unary operator", expr.Operator.Line);
    }

    public object? VisitVariableExpr(Expr.Variable expr)
    {
        if (!Scope.TryGetVariable(expr.Name.Lexeme, out var variable))
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
        if (Scope.Enclosing == null)
        {
            Scope.InitializingFunctions.Add(RootNamespace + ":" + Path.Combine(Scope.Namespace, "_init"));
        }
        
        CompileBlock(stmt.Statements, Scope.CurrentFunction);
        return null;
    }

    public object? VisitExpressionStmt(Stmt.Expression stmt)
    {
        Evaluate(stmt.Expr);
        return null;
    }

    public object? VisitNamespaceStmt(Stmt.Namespace stmt)
    {
        var filePath = Path.Combine(OutDir, RootNamespace, "functions", Scope.Namespace, stmt.Name.Lexeme);
        Directory.CreateDirectory(filePath);
        CompileBlock(stmt.Body, scope: stmt.Name.Lexeme);
        return null;
    }

    public object? VisitFunctionStmt(Stmt.Function stmt)
    {
        // var variable = Environment.AddVariable(stmt.Name.Lexeme, Subject.Storage);
        // Todo: add function to environment
        var functionPath = RootNamespace + ":" + Path.Combine(Scope.Namespace, stmt.Name.Lexeme);
        
        if (stmt.Initializing)
        {
            Scope.InitializingFunctions.Add(functionPath);
        }

        if (stmt.Ticking)
        {
            Scope.TickingFunctions.Add(functionPath);
        }
        
        var previous = Scope;
        Scope = new Environment(Scope, stmt.Name.Lexeme);
        CompileBlock(stmt.Body, stmt.Name.Lexeme);
        Scope = previous;
        return null;
    }

    public object? VisitIfStmt(Stmt.If stmt)
    {
        var functionDirectory = Path.Combine(OutDir, RootNamespace, "functions", Scope.Namespace, Scope.CurrentFunction);
        Directory.CreateDirectory(functionDirectory);

        var depth = Scope.IfCounter++;
        
        var ifFunctionPath = RootNamespace + ":" + Path.Combine(Scope.Namespace, Scope.CurrentFunction, $"_if{depth}");
        AddCommand($"execute if function {ifFunctionPath} run return 1");
        AddCommand("execute if score _brk amethyst matches 1 run return fail");
        var controlFlowEnvironment = Scope;
        Scope = new Environment(Scope, $"_if{depth}", Scope.CurrentFunction);
        {
            var controlFlowDirectory = Path.Combine(OutDir, RootNamespace, "functions", Scope.Namespace, Scope.CurrentFunction);
            Directory.CreateDirectory(controlFlowDirectory);
            Evaluate(stmt.Condition);
            
            var thenEnvironment = Scope;
            Scope = new Environment(Scope, "_then", Scope.CurrentFunction);
            var thenFunctionPath = RootNamespace + ":" + Path.Combine(Scope.Namespace, Scope.CurrentFunction);
            Compile(stmt.ThenBranch);
            Scope = thenEnvironment;
            
            AddCommand($"execute if score _out amethyst matches 1 run return run function {thenFunctionPath}");
            
            var elseEnvironment = Scope;
            Scope = new Environment(Scope, "_else", Scope.CurrentFunction);
            var elseFunctionPath = RootNamespace + ":" + Path.Combine(Scope.Namespace, Scope.CurrentFunction);
            if (stmt.ElseBranch != null)
            {
                Compile(stmt.ElseBranch);
            }
            Scope = elseEnvironment;
            
            if (stmt.ElseBranch != null)
            {
                AddCommand($"return run function {elseFunctionPath}");
            }
        }
        Scope = controlFlowEnvironment;
        return null;
    }

    public object? VisitPrintStmt(Stmt.Print stmt)
    {
        if (Evaluate(stmt.Expr) is Subject subject)
        {
            switch (subject)
            {
                case Subject.Scoreboard:
                    AddCommand("execute store result storage amethyst:internal _out float 0.01 run scoreboard players get _out amethyst");
                    break;
                case Subject.Storage:
                    break;
            }
            AddCommand("tellraw @a {\"storage\":\"amethyst:internal\",\"nbt\":\"_out\"}");
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
        if (stmt.Value != null)
        {
            Evaluate(stmt.Value);
        }
        AddCommand("return 1");
        return null;
    }

    public object? VisitBreakStmt(Stmt.Break stmt)
    {
        AddCommand("scoreboard players set _brk amethyst 1");
        AddCommand("return fail");
        return null;
    }

    public object? VisitContinueStmt(Stmt.Continue stmt)
    {
        AddCommand("scoreboard players set _brk amethyst 0");
        AddCommand("return fail");
        return null;
    }

    public object? VisitVarStmt(Stmt.Var stmt)
    {
        if (!Scope.AddVariable(stmt.Name.Lexeme, Subject.Storage /* temporary; getting overriden later */, out var variable))
        {
            throw new SyntaxException($"Variable {stmt.Name.Lexeme} already defined", stmt.Name.Line);
        }

        if (Evaluate(stmt.Initializer) is Subject subject)
        {
            variable.Subject = subject;
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
        var functionDirectory = Path.Combine(OutDir, RootNamespace, "functions", Scope.Namespace, Scope.CurrentFunction);
        Directory.CreateDirectory(functionDirectory);

        var depth = Scope.LoopCounter++;
        
        var loopFunctionPath = RootNamespace + ":" + Path.Combine(Scope.Namespace, Scope.CurrentFunction, $"_loop{depth}");
        AddCommand($"execute if function {loopFunctionPath} run return 1");
        var loopEnvironment = Scope;
        Scope = new Environment(Scope, $"_loop{depth}", Scope.CurrentFunction);
        {
            if (Evaluate(stmt.Condition) is Subject.Scoreboard)
            {
                AddCommand("execute if score _out amethyst matches 0 run return fail");
            }
            Compile(stmt.Body);
            AddCommand($"return run function {loopFunctionPath}");
        }
        Scope = loopEnvironment;
        return null;
    }
}