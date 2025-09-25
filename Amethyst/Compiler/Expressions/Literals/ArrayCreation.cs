using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractValue VisitArrayCreation(AmethystParser.ArrayCreationContext context)
    {
        DataType? dataType = null;
        var isDynamic = false;
        var elements = new List<AbstractValue>();

        var arrayElementContexts = context.arrayElement();
        ProcessArrayElements(arrayElementContexts);

        if (dataType == null)
        {
            isDynamic = true;
        }
        
        if (elements.All(element => element is IConstantValue))
        {
            var value = elements.Cast<IConstantValue>().ToArray();
            
            if (isDynamic)
            {
                return new ConstantDynamicArray
                {
                    Compiler = this,
                    Context = context,
                    Value = value,
                };
            }

            return new ConstantStaticArray
            {
                Compiler = this,
                Context = context,
                Value = value,
                BasicType = dataType!.BasicType
            };
        }

        var parts = new List<IConstantValue>();
        var substitutions = new List<KeyValuePair<object, IRuntimeValue>>();

        for (var i = 0; i < elements.Count; i++)
        {
            var element = elements[i];
            
            if (element is IConstantValue constantValue)
            {
                parts.Add(constantValue);
            }
            if (element is IRuntimeValue runtimeValue)
            {
                parts.Add(runtimeValue.AsConstantSubstitute);

                substitutions.Add(new KeyValuePair<object, IRuntimeValue>(i, runtimeValue));
            }
        }
        
        if (isDynamic)
        {
            return new ConstantDynamicArray
            {
                Compiler = this,
                Context = context,
                Value = parts.ToArray(),
                Substitutions = substitutions
            };
        }

        return new ConstantStaticArray
        {
            Compiler = this,
            Context = context,
            Value = parts.ToArray(),
            Substitutions = substitutions,
            BasicType = dataType!.BasicType
        };

        void ProcessArrayElements(IEnumerable<AmethystParser.ArrayElementContext> arrayElementContexts)
        {
            foreach (var arrayElementContext in arrayElementContexts)
            {
                if (arrayElementContext.preprocessorYieldingStatement() is { } preprocessorYieldingStatementContext)
                {
                    var result = VisitPreprocessorYieldingStatement<AmethystParser.ArrayElementContext>(preprocessorYieldingStatementContext);
                    ProcessArrayElements(result);
                }
                else if (arrayElementContext.expression() is { } expressionContext)
                {
                    var result = VisitExpression(expressionContext);
                
                    // if the initial data type differs from the current data type, we need to make the array dynamic
                    if (!isDynamic && dataType != null && dataType != result.DataType)
                    {
                        isDynamic = true;
                    }

                    // set the initial data type if it is not set yet
                    if (!isDynamic)
                    {
                        dataType ??= result.DataType;
                    }
                    
                    elements.Add(result);
                }
            }
        }
    }
}