using Amethyst.Language;
using Amethyst.Utility;

namespace Amethyst;

public partial class Compiler
{
    /// <inheritdoc />
    /// <summary>
    ///     <p>An object creation expression. A static object has a fixed datatype, while a dynamic object
    ///     can have elements of different datatypes. This is important when e.g. iterating over an object
    ///     to know the datatype of the elements.</p>
    ///     <p>If the object contains runtime elements, they are substituted with constant placeholder
    ///     elements that point to the runtime data. When generating the actual code for the object
    ///     creation, these placeholder elements are then replaced by their actual runtime value.</p>
    ///     <p><inheritdoc /></p></summary>
    public override AbstractValue VisitObjectLiteral(AmethystParser.ObjectLiteralContext context)
    {
        var objectElementContexts = context.objectElement();
        var elements = new Dictionary<AbstractString, AbstractValue>();
        ProcessObjectElements(objectElementContexts, ref elements, out var isDynamic, out var dataType);
        var hasMacroPlaceholders = false;
        
        if (dataType == null)
        {
            isDynamic = true;
        }
        
        if (elements.Keys.All(key => key is ConstantString) && elements.Values.All(value => value is IConstantValue))
        {
            var value = elements.ToDictionary(
                kvp => ((ConstantString)kvp.Key).Value, 
                kvp => (IConstantValue)kvp.Value);
            
            if (isDynamic)
            {
                return new ConstantDynamicObject
                {
                    Compiler = this,
                    Context = context,
                    Value = value,
                    HasMacroPlaceholders = hasMacroPlaceholders
                };
            }

            return new ConstantStaticObject
            {
                Compiler = this,
                Context = context,
                Value = value,
                BasicType = dataType!.BasicType,
                HasMacroPlaceholders = hasMacroPlaceholders
            };
        }
        
        var parts = new Dictionary<string, IConstantValue>();
        var substitutions = new List<KeyValuePair<string, IRuntimeValue>>();

        foreach (var element in elements)
        {
            string key = null!;
            if (element.Key is ConstantString constantKey)
            {
                key = constantKey.Value;
            }
            else if (element.Key is RuntimeString runtimeKey)
            {
                key = runtimeKey.Location.ToMacroPlaceholder();
                hasMacroPlaceholders = true;
            }
            
            if (element.Value is IConstantValue constantValue)
            {
                parts.Add(key, constantValue);
            }
            if (element.Value is IRuntimeValue runtimeValue)
            {
                parts.Add(key, runtimeValue.AsConstantSubstitute);

                substitutions.Add(new KeyValuePair<string, IRuntimeValue>(key, runtimeValue));
            }
        }
        
        if (isDynamic)
        {
            return new ConstantDynamicObject
            {
                Compiler = this,
                Context = context,
                Value = parts,
                Substitutions = substitutions,
                HasMacroPlaceholders = hasMacroPlaceholders
            };
        }

        return new ConstantStaticObject
        {
            Compiler = this,
            Context = context,
            Value = parts,
            Substitutions = substitutions,
            BasicType = dataType!.BasicType,
            HasMacroPlaceholders = hasMacroPlaceholders
        };
    }

    public override KeyValuePairClass<AbstractString, AbstractValue> VisitObjectKvp(AmethystParser.ObjectKvpContext context)
    {
        var key = (AbstractString)Visit(context.stringLiteral())!;
        var result = VisitExpression(context.expression());
        return new KeyValuePairClass<AbstractString, AbstractValue>(key, result);
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
    private void ProcessObjectElements(IEnumerable<AmethystParser.ObjectElementContext> objectElementContexts, ref Dictionary<AbstractString, AbstractValue> elements, out bool isDynamic, out AbstractDatatype? dataType)
    {
        dataType = null;
        isDynamic = false;
        
        foreach (var objectElementContext in objectElementContexts)
        {
            if (objectElementContext.preprocessorYieldingStatement() is { } preprocessorYieldingStatementContext)
            {
                foreach (var kvp in VisitPreprocessorYieldingStatement<KeyValuePairClass<AbstractString, AbstractValue>>(preprocessorYieldingStatementContext))
                {
                    ProcessObjectElement(kvp, elements, ref isDynamic, ref dataType, objectElementContext);
                }
            }
            else if (objectElementContext.objectKvp() is { } objectKvpContext)
            {
                var kvp = VisitObjectKvp(objectKvpContext);

                ProcessObjectElement(kvp, elements, ref isDynamic, ref dataType, objectElementContext);
            }
        }
    }

    private static void ProcessObjectElement(KeyValuePairClass<AbstractString, AbstractValue> kvp, Dictionary<AbstractString, AbstractValue> elements, ref bool isDynamic, ref AbstractDatatype? dataType, AmethystParser.ObjectElementContext objectElementContext)
    {
        if (kvp.Key is ConstantString constantKey && elements.Keys.Any(k => k is ConstantString constantString && constantString.Value == constantKey.Value))
        {
            throw new SyntaxException($"Duplicate key '{kvp.Key}'.", objectElementContext);
        }
            
        // if the initial data type differs from the current data type, we need to make the array dynamic
        if (!isDynamic && dataType != null && dataType != kvp.Value.Datatype)
        {
            isDynamic = true;
        }

        // set the initial data type if it is not set yet
        if (!isDynamic)
        {
            dataType ??= kvp.Value.Datatype;
        }
            
        elements.Add(kvp.Key, kvp.Value);
    }
}