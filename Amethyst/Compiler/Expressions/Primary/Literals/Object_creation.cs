using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;

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
            var key = objectElementContext.IDENTIFIER().Symbol.Text;
            
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
        
        var parts = new Dictionary<string, ConstantValue>();
        var substitutions = new List<KeyValuePair<object, RuntimeValue>>();

        foreach (var kvp in map)
        {
            if (kvp.Value is ConstantValue constantValue)
            {
                parts.Add(kvp.Key, constantValue);
            }
            if (kvp.Value is RuntimeValue runtimeValue)
            {
                parts.Add(kvp.Key, runtimeValue.ToConstantSubstitute());

                substitutions.Add(new KeyValuePair<object, RuntimeValue>(kvp.Key, runtimeValue));
            }
        }
        
        if (isDynamic)
        {
            return new DynObjectConstant
            {
                Compiler = this,
                Context = context,
                Value = parts,
                Substitutions = substitutions
            };
        }

        return new StaticObjectConstant
        {
            Compiler = this,
            Context = context,
            Value = parts,
            Substitutions = substitutions,
            BasicType = dataType!.BasicType
        };
    }
}