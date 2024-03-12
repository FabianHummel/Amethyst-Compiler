namespace Amethyst;

public static class Generator
{
    public static void Generate(IEnumerable<AstNode> nodes, string rootNamespace, string outDir)
    {
        var context = new AstContext
        {
            Namespace = rootNamespace,
            OutDir = outDir + "/data/" + rootNamespace + "/functions",
            CurrentFunction = null,
            Functions = new List<FunctionDefinition>(),
            Variables = new List<VariableDefinition>()
        };
        foreach (var node in nodes)
        {
            node.GenerateCode(context);
        }
    }
}

public partial interface AstNode
{
    void GenerateCode(AstContext context);
}

public partial class Variable
{
    public void GenerateCode(AstContext context)
    {
        context.Variables.Add(new VariableDefinition
        {
            Name = Name,
            Type = Type
        });
    }
}

public partial class Assignment
{
    public void GenerateCode(AstContext context)
    {
        
    }
}

public partial class Function
{
    public void GenerateCode(AstContext context)
    {
        context.Functions.Add(new FunctionDefinition
        {
            Name = Name,
            ReturnType = "void", // TODO: infer return type
            Parameters = Arguments
        });
        
        File.Create(Path.Combine(context.OutDir, Name + ".mcfunction"));
        
        var scope = new AstContext
        {
            Namespace = context.Namespace,
            OutDir = context.OutDir,
            CurrentFunction = Name,
            Variables = new List<VariableDefinition>(context.Variables),
            Functions = new List<FunctionDefinition>(context.Functions)
        };
        
        foreach (var node in Body)
        {
            node.GenerateCode(scope);
        }
    }
}

public partial class FunctionDecorators
{
    public void GenerateCode(AstContext context)
    {
        // if ticking, add to tick.json
        // if initializing, add to load.json
    }
}

public abstract partial class Identifier
{
    public void GenerateCode(AstContext context)
    {
        
    }
}

public abstract partial class Arguments
{
    public void GenerateCode(AstContext context)
    {
        
    }
}

public abstract partial class Parameters
{
    public void GenerateCode(AstContext context)
    {
        
    }
}

public abstract partial class Body
{
    public void GenerateCode(AstContext context)
    {
        
    }
}

public partial class Namespace
{
    public void GenerateCode(AstContext context)
    {
        var dir = Path.Combine(context.OutDir, Name);
        Directory.CreateDirectory(dir);
        
        var scope = new AstContext
        {
            Namespace = context.Namespace + Path.PathSeparator + Name,
            OutDir = dir,
            CurrentFunction = context.CurrentFunction,
            Variables = new List<VariableDefinition>(context.Variables),
            Functions = new List<FunctionDefinition>(context.Functions)
        };
        
        foreach (var node in Body)
        {
            node.GenerateCode(scope);
        }
    }
}

public partial class FunctionCall
{
    public void GenerateCode(AstContext context)
    {
        
    }
}

public partial class VariableReference
{
    public void GenerateCode(AstContext context)
    {
        
    }
}


public partial class Constant
{
    public void GenerateCode(AstContext context)
    {
        
    }
}

public partial class Operation
{
    public void GenerateCode(AstContext context)
    {
        
    }
}