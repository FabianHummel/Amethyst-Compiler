using System.Text.Json;

namespace Amethyst;

public static class Generator
{
    public static void Generate(IEnumerable<AstNode> nodes, string rootNamespace, string outDir)
    {
        var context = new GenerationContext
        {
            RootNamespace = rootNamespace,
            CurrentNamespace = "",
            OutDir = outDir + "/data/" + rootNamespace + "/functions",
            CurrentFunction = null,
            Functions = new List<FunctionDefinition>(),
            Variables = new List<VariableDefinition>(),
            TickFunctions = new List<string>(),
            LoadFunctions = new List<string>()
        };
        foreach (var node in nodes)
        {
            node.GenerateCode(context);
        }

        Project.CreateFunctionTags(outDir, context.TickFunctions, context.LoadFunctions);
    }
}

public partial interface AstNode
{
    void GenerateCode(GenerationContext context);
}

public partial class Variable
{
    public void GenerateCode(GenerationContext context)
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
    public void GenerateCode(GenerationContext context)
    {
        
    }
}

public partial class Function
{
    public void GenerateCode(GenerationContext context)
    {
        context.Functions.Add(new FunctionDefinition
        {
            Name = context.RootNamespace + ":" + Path.Combine(context.CurrentNamespace, Name),
            ReturnType = "void", // TODO: infer return type
            Parameters = Arguments
        });
        
        var path = Path.Combine(context.OutDir, context.CurrentNamespace, Name + ".mcfunction");
        File.Create(path);
        
        var currentFunction = context.CurrentFunction;
        context.CurrentFunction = Name;
        
        Decorators.GenerateCode(context);
        
        foreach (var node in Body)
        {
            node.GenerateCode(context);
        }
        
        context.CurrentFunction = currentFunction;
    }
}

public partial class FunctionDecorators
{
   
    public void GenerateCode(GenerationContext context)
    {
        var path = context.RootNamespace + ":" + Path.Combine(context.CurrentNamespace, context.CurrentFunction!);
        
        if (IsTicking)
        {
            context.TickFunctions.Add(path);
        }

        if (IsInitializing)
        {
            context.LoadFunctions.Add(path);
        }
    }
}

public abstract partial class Identifier
{
    public void GenerateCode(GenerationContext context)
    {
        
    }
}

public abstract partial class Arguments
{
    public void GenerateCode(GenerationContext context)
    {
        
    }
}

public abstract partial class Parameters
{
    public void GenerateCode(GenerationContext context)
    {
        
    }
}

public abstract partial class Body
{
    public void GenerateCode(GenerationContext context)
    {
        
    }
}

public partial class Namespace
{
    public void GenerateCode(GenerationContext context)
    {
        var dir = Path.Combine(context.OutDir, context.CurrentNamespace, Name);
        Directory.CreateDirectory(dir);
        
        var @namespace = context.CurrentNamespace;
        context.CurrentNamespace = Path.Combine(context.CurrentNamespace, Name);
        
        foreach (var node in Body)
        {
            node.GenerateCode(context);
        }
        
        context.CurrentNamespace = @namespace;
    }
}

public partial class FunctionCall
{
    public void GenerateCode(GenerationContext context)
    {
        
    }
}

public partial class VariableReference
{
    public void GenerateCode(GenerationContext context)
    {
        
    }
}


public partial class Constant
{
    public void GenerateCode(GenerationContext context)
    {
        
    }
}

public partial class Operation
{
    public void GenerateCode(GenerationContext context)
    {
        
    }
}