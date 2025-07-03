using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;
using Amethyst.Utility;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitObject_creation(AmethystParser.Object_creationContext context)
    {
        if (context.object_element() is not { } objectElementContexts)
        {
            throw new UnreachableException();
        }
        
        DataType? dataType = null;
        
        var dynamic = false;
        
        var map = new Dictionary<string, AbstractResult>();

        foreach (var objectElementContext in objectElementContexts)
        {
            var key = objectElementContext.identifier().IDENTIFIER().Symbol.Text;
            
            var result = VisitExpression(objectElementContext.expression());
            
            
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
            
            map.Add(key, result);
        }
        
        var isDynamic = dynamic || dataType == null;
        
        if (map.All(element => element.Value is ConstantValue))
        {
            var value = map.ToDictionary(
                kvp => kvp.Key, 
                kvp => (ConstantValue)kvp.Value);
            
            if (isDynamic)
            {
                return new DynObjectConstant
                {
                    Compiler = this,
                    Context = context,
                    Value = value,
                };
            }

            return new StaticObjectConstant
            {
                Compiler = this,
                Context = context,
                Value = value,
                BasicType = dataType!.BasicType
            };
        }
        
        var parts = new Dictionary<string, string>();
        var substitutions = new List<KeyValuePair<object, RuntimeValue>>();

        foreach (var kvp in map)
        {
            if (kvp.Value is ConstantValue constantValue)
            {
                parts.Add(kvp.Key, constantValue.ToNbtString());
            }
            if (kvp.Value is RuntimeValue runtimeValue)
            {
                parts.Add(kvp.Key, $"{runtimeValue.DataType.DefaultValue}");

                substitutions.Add(new KeyValuePair<object, RuntimeValue>(kvp.Key, runtimeValue));
            }
        }
        
        var location = ++StackPointer;
            
        AddCode($"data modify storage amethyst: {location} set value {{" +
                $"keys:[{string.Join(",", parts.Keys.Select(key => key.ToNbtString()))}]," +
                $"data:{{{string.Join(',', parts.Select(kvp => $"{kvp.Key.ToNbtString()}:{kvp.Value}"))}}}}}");
        
        ObjectBase obj;
        
        if (isDynamic)
        {
            obj = new DynObjectResult
            {
                Compiler = this,
                Context = context,
                Location = location.ToString(),
                Substitutions = substitutions
            };
        }
        else
        {
            obj = new StaticObjectResult
            {
                Compiler = this,
                Context = context,
                Location = location.ToString(),
                Substitutions = substitutions,
                BasicType = dataType!.BasicType
            };
        }
        
        obj.SubstituteRecursively($"{location}.data");

        return obj;
    }
}