using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitArray_creation(AmethystParser.Array_creationContext context)
    {
        if (context.expression() is not { } expressionContexts)
        {
            throw new UnreachableException();
        }
        
        if (expressionContexts.Length == 0)
        {
            AddCode($"data modify storage amethyst: {MemoryLocation} set value [];");
        }
        
        DataType? dataType = null;
        
        var dynamic = false;

        var elements = new List<AbstractResult>(expressionContexts.Select(expressionContext =>
        {
            var result = VisitExpression(expressionContext);
            
            if (!dynamic && dataType != null && dataType != result.DataType)
            {
                dynamic = true;
            }

            if (!dynamic)
            {
                dataType ??= result.DataType;
            }
            
            return result;
        }));
        
        if (dataType == null)
        {
            return CreateDynamicArray(context, elements);
        }

        if (!dynamic)
        {
            return CreateStaticArray(context, dataType, elements);
        }

        return CreateDynamicArray(context, elements);
    }
    
    public ArrayResult CreateStaticArray(AmethystParser.Array_creationContext context, DataType dataType, List<AbstractResult> elements)
    {
        var parts = new List<string>();
        
        var scope = EvaluateScoped("_array", () =>
        {
            foreach (var element in elements)
            {
                if (element.ConstantValue is { } constantValue)
                {
                    parts.Add(constantValue.ToString()!);
                }
                else if (element.Location is { } location)
                {
                    if (element.DataType.IsStorageType)
                    {
                        AddCode($"data modify storage amethyst: {MemoryLocation}[{parts.Count}] set from storage amethyst: {location}");
                    }
                    else if (element.DataType.IsScoreboardType)
                    {
                        AddCode($"execute store result storage amethyst: {MemoryLocation}[{parts.Count}] {dataType.StorageModifier} run scoreboard players get {location} amethyst");
                    }
                    
                    parts.Add(element.DataType.DefaultValue);
                }
                else
                {
                    throw new UnreachableException();
                }
            }
        });
        
        AddCode($"data modify storage amethyst: {MemoryLocation} set value [{string.Join(',', parts)}]");
        AddCode($"function {scope.McFunctionPath}");
        
        return new()
        {
            Compiler = this,
            BasicType = dataType.BasicType,
            Location = MemoryLocation++.ToString(),
            Context = context
        };
    }
    
    public DynArrayResult CreateDynamicArray(AmethystParser.Array_creationContext context, List<AbstractResult> elements)
    {
        var parts = new List<string>();
        
        var scope = EvaluateScoped("_array", () =>
        {
            foreach (var element in elements)
            {
                if (element.ConstantValue is { } constantValue)
                {
                    parts.Add($"{{_:{constantValue}}}");
                }
                else if (element.Location is { } location)
                {
                    if (element.DataType.IsStorageType)
                    {
                        AddCode($"data modify storage amethyst: {MemoryLocation}[{parts.Count}]._ set from storage amethyst: {location}");
                    }
                    else if (element.DataType.IsScoreboardType)
                    {
                        AddCode($"execute store result storage amethyst: {MemoryLocation}[{parts.Count}]._ {element.DataType.StorageModifier} run scoreboard players get {location} amethyst");
                    }
                    
                    parts.Add(BasicType.Array.GetDefaultValue());
                }
                else
                {
                    throw new UnreachableException();
                }
            }
        });
        
        AddCode($"data modify storage amethyst: {MemoryLocation} set value [{string.Join(',', parts)}]");
        AddCode($"function {scope.McFunctionPath}");
        
        return new()
        {
            Compiler = this,
            Location = MemoryLocation++.ToString(),
            Context = context
        };
    }
}