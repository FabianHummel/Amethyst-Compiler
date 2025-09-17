using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractResult VisitArrayCreation(AmethystParser.ArrayCreationContext context)
    {
        DataType? dataType = null;
        var isDynamic = false;
        var elements = new List<AbstractResult>();

        var arrayElementContexts = context.arrayElement();
        ProcessArrayElements(arrayElementContexts);

        if (dataType == null)
        {
            isDynamic = true;
        }
        
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