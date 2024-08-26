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

        var substitutions = new List<KeyValuePair<object, AbstractResult>>();

        for (var i = 0; i < elements.Count; i++)
        {
            var element = elements[i];
            
            if (element.ConstantValue is { } constantValue)
            {
                parts.Add($"{constantValue}");

                if (element.Substitutions != null)
                {
                    substitutions.Add(new KeyValuePair<object, AbstractResult>(i, element));
                }
            }
            else if (element.Location is not null)
            {
                parts.Add($"{element.DataType.DefaultValue}");

                substitutions.Add(new KeyValuePair<object, AbstractResult>(i, element));
            }
            else
            {
                throw new UnreachableException();
            }
        }

        return new ArrayResult
        {
            Compiler = this,
            BasicType = dataType.BasicType,
            Context = context,
            ConstantValue = $"[{string.Join(',', parts)}]",
            Substitutions = substitutions
        };
    }
    
    public DynArrayResult CreateDynamicArray(AmethystParser.Array_creationContext context, List<AbstractResult> elements)
    {
        var parts = new List<string>();
        
        var substitutions = new List<KeyValuePair<object, AbstractResult>>();

        for (var i = 0; i < elements.Count; i++)
        {
            var element = elements[i];
            
            if (element.ConstantValue is { } constantValue)
            {
                parts.Add($"{{_:{constantValue}}}");

                if (element.Substitutions != null)
                {
                    substitutions.Add(new KeyValuePair<object, AbstractResult>(i, element));
                }
            }
            else if (element.Location is not null)
            {
                parts.Add($"{BasicType.Array.GetDefaultValue()}");

                substitutions.Add(new KeyValuePair<object, AbstractResult>(i, element));
            }
            else
            {
                throw new UnreachableException();
            }
        }

        return new DynArrayResult
        {
            Compiler = this,
            Context = context,
            ConstantValue = $"[{string.Join(',', parts)}]",
            Substitutions = substitutions
        };
    }
}