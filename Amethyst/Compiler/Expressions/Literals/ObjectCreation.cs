using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    /// <summary>
    ///     <p>An object creation expression. A static object has a fixed datatype, while a dynamic object
    ///     can have elements of different datatypes. This is important when e.g. iterating over an object
    ///     to know the datatype of the elements.</p>
    ///     <p>If the object contains runtime elements, they are substituted with constant placeholder
    ///     elements that point to the runtime data. When generating the actual code for the object
    ///     creation, these placeholder elements are then replaced by their actual runtime value.</p>
    ///     <p><inheritdoc /></p></summary>
    public override AbstractValue VisitObjectCreation(AmethystParser.ObjectCreationContext context)
    {
        var objectElementContexts = context.objectElement();
        var elements = new Dictionary<string, AbstractValue>();
        ProcessObjectElements(objectElementContexts, ref elements, out var isDynamic, out var dataType);
        
        if (dataType == null)
        {
            isDynamic = true;
        }
        
        if (elements.Values.All(element => element is IConstantValue))
        {
            var value = elements.ToDictionary(
                kvp => kvp.Key, 
                kvp => (IConstantValue)kvp.Value);
            
            if (isDynamic)
            {
                return new ConstantDynamicObject
                {
                    Compiler = this,
                    Context = context,
                    Value = value
                };
            }

            return new ConstantStaticObject
            {
                Compiler = this,
                Context = context,
                Value = value,
                BasicType = dataType!.BasicType
            };
        }
        
        var parts = new Dictionary<string, IConstantValue>();
        var substitutions = new List<KeyValuePair<object, IRuntimeValue>>();

        foreach (var element in elements)
        {
            if (element.Value is IConstantValue constantValue)
            {
                parts.Add(element.Key, constantValue);
            }
            if (element.Value is IRuntimeValue runtimeValue)
            {
                parts.Add(element.Key, runtimeValue.AsConstantSubstitute);

                substitutions.Add(new KeyValuePair<object, IRuntimeValue>(element.Key, runtimeValue));
            }
        }
        
        if (isDynamic)
        {
            return new ConstantDynamicObject
            {
                Compiler = this,
                Context = context,
                Value = parts,
                Substitutions = substitutions
            };
        }

        return new ConstantStaticObject
        {
            Compiler = this,
            Context = context,
            Value = parts,
            Substitutions = substitutions,
            BasicType = dataType!.BasicType
        };
    }

    /// <summary>
    ///     <p>Processes the object elements and determines the datatype and if the object is ultimately
    ///     dynamic.</p>
    /// </summary>
    /// <param name="objectElementContexts">The individual AST nodes of the array elements to process.</param>
    /// <param name="elements">The dictionary to fill with the processed elements.</param>
    /// <param name="isDynamic">Whether the object is dynamic.</param>
    /// <param name="dataType">The datatype of the object elements, if static.</param>
    /// <exception cref="SyntaxException">Thrown if a duplicate key is found.</exception>
    private void ProcessObjectElements(IEnumerable<AmethystParser.ObjectElementContext> objectElementContexts, ref Dictionary<string, AbstractValue> elements, out bool isDynamic, out AbstractDatatype? dataType)
    {
        dataType = null;
        isDynamic = false;
        
        foreach (var objectElementContext in objectElementContexts)
        {
            if (objectElementContext.preprocessorYieldingStatement() is { } preprocessorYieldingStatementContext)
            {
                var result = VisitPreprocessorYieldingStatement<AmethystParser.ObjectElementContext>(preprocessorYieldingStatementContext);
                ProcessObjectElements(result, ref elements, out isDynamic, out dataType);
            }
            else if (objectElementContext.objectKvp() is { } objectKvpContext)
            {
                var key = objectKvpContext.IDENTIFIER().Symbol.Text;
                var result = VisitExpression(objectKvpContext.expression());
                
                if (elements.ContainsKey(key))
                {
                    throw new SyntaxException($"Duplicate key '{key}'.", objectElementContext);
                }
            
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
            
                elements.Add(key, result);
            }
        }
    }
}