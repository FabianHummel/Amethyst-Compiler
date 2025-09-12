using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitArrayCreation(AmethystParser.ArrayCreationContext context)
    {
        var expressionContexts = context.expression();
        
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

        var parts = new List<ConstantValue>();
        var substitutions = new List<KeyValuePair<object, RuntimeValue>>();

        for (var i = 0; i < elements.Count; i++)
        {
            var element = elements[i];
            
            if (element is ConstantValue constantValue)
            {
                parts.Add(constantValue);
            }
            if (element is RuntimeValue runtimeValue)
            {
                parts.Add(runtimeValue.ToConstantSubstitute());

                substitutions.Add(new KeyValuePair<object, RuntimeValue>(i, runtimeValue));
            }
        }
        
        if (isDynamic)
        {
            return new DynArrayConstant
            {
                Compiler = this,
                Context = context,
                Value = parts.ToArray(),
                Substitutions = substitutions
            };
        }

        return new StaticArrayConstant
        {
            Compiler = this,
            Context = context,
            Value = parts.ToArray(),
            Substitutions = substitutions,
            BasicType = dataType!.BasicType
        };
    }
}