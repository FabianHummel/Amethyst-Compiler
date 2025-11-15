using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    /// <inheritdoc />
    /// <summary>
    ///     <p>An array creation expression. A static array has a fixed datatype, while a dynamic array can
    ///     have elements of different datatypes. This is important when e.g. iterating over an array to
    ///     know the datatype of the elements.</p>
    ///     <p>If the array contains runtime elements, they are substituted with constant placeholder
    ///     elements that point to the runtime data. When generating the actual code for the array
    ///     creation, these placeholder elements are then replaced by their actual runtime value.</p>
    ///     <p><inheritdoc /></p></summary>
    public override AbstractValue VisitArrayCreation(AmethystParser.ArrayCreationContext context)
    {
        var arrayElementContexts = context.arrayElement();
        var elements = ProcessArrayElements(arrayElementContexts, out var isDynamic, out var dataType);

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
                    Value = value
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
    }

    /// <summary>
    ///     <p>Processes the array elements and determines the datatype and if the array is ultimately
    ///     dynamic.</p>
    /// </summary>
    /// <param name="arrayElementContexts">The individual AST nodes of the array elements to process.</param>
    /// <param name="isDynamic">Whether the array is dynamic.</param>
    /// <param name="dataType">The datatype of the array elements, if static.</param>
    /// <returns></returns>
    private List<AbstractValue> ProcessArrayElements(IEnumerable<AmethystParser.ArrayElementContext> arrayElementContexts, out bool isDynamic, out AbstractDatatype? dataType)
    {
        var elements = new List<AbstractValue>();
        isDynamic = false;
        dataType = null;
        
        foreach (var arrayElementContext in arrayElementContexts)
        {
            if (arrayElementContext.preprocessorYieldingStatement() is { } preprocessorYieldingStatementContext)
            {
                var result = VisitPreprocessorYieldingStatement<AmethystParser.ArrayElementContext>(preprocessorYieldingStatementContext);
                elements.AddRange(ProcessArrayElements(result, out isDynamic, out dataType));
            }
            else if (arrayElementContext.expression() is { } expressionContext)
            {
                var result = VisitExpression(expressionContext);
                
                // if the initial data type differs from the current data type, we need to make the array dynamic
                if (!isDynamic && dataType != null && dataType != result.Datatype)
                {
                    isDynamic = true;
                }

                // set the initial data type if it is not set yet
                if (!isDynamic)
                {
                    dataType ??= result.Datatype;
                }
                
                elements.Add(result);
            }
        }
        
        return elements;
    }
}