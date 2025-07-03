using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitArray_creation(AmethystParser.Array_creationContext context)
    {
        if (context.expression() is not { } expressionContexts)
        {
            throw new UnreachableException();
        }
        
        DataType? dataType = null;
        
        var dynamic = false;

        var elements = new List<AbstractResult>();
        
        foreach (var expressionContext in expressionContexts)
        {
            var result = VisitExpression(expressionContext);
            
            // if the initial data type differs from the current data type, we need to make the array dynamic
            if (!dynamic && dataType != null && dataType != result.DataType)
            {
                dynamic = true;
            }

            // set the initial data type if it is not set yet
            if (!dynamic)
            {
                dataType ??= result.DataType;
            }
            
            elements.Add(result);
        }

        var isDynamic = dynamic || dataType == null;
        
        if (elements.All(element => element is ConstantValue))
        {
            var value = elements.Cast<ConstantValue>().ToArray();
            
            if (isDynamic)
            {
                return new DynArrayConstant
                {
                    Compiler = this,
                    Context = context,
                    Value = value,
                };
            }

            return new StaticArrayConstant
            {
                Compiler = this,
                Context = context,
                Value = value,
                BasicType = dataType!.BasicType
            };
        }

        var parts = new List<string>();
        var substitutions = new List<KeyValuePair<object, RuntimeValue>>();

        for (var i = 0; i < elements.Count; i++)
        {
            var element = elements[i];
            
            if (element is ConstantValue constantValue)
            {
                parts.Add(constantValue.ToNbtString());
            }
            if (element is RuntimeValue runtimeValue)
            {
                parts.Add($"{runtimeValue.DataType.DefaultValue}");

                substitutions.Add(new KeyValuePair<object, RuntimeValue>(i, runtimeValue));
            }
        }

        var location = ++StackPointer;
            
        ArrayBase array;
            
        if (isDynamic)
        {
            AddCode($"data modify storage amethyst: {location} set value [{string.Join(',', $"{{_:{parts}}}")}]");
                
            array = new DynArrayResult
            {
                Compiler = this,
                Context = context,
                Location = location.ToString(),
                Substitutions = substitutions
            };
        }
        else
        {
            AddCode($"data modify storage amethyst: {location} set value [{string.Join(',', parts)}]");
            
            array = new StaticArrayResult
            {
                Compiler = this,
                Context = context,
                Location = location.ToString(),
                Substitutions = substitutions,
                BasicType = dataType!.BasicType
            };
        }

        array.SubstituteRecursively(location.ToString());

        return array;
    }
}