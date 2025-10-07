using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override AbstractValue VisitObjectCreation(AmethystParser.ObjectCreationContext context)
    {
        AbstractDatatype? dataType = null;
        var dynamic = false;
        var map = new Dictionary<string, AbstractValue>();
        
        var objectElementContexts = context.objectElement();
        ProcessObjectElements(objectElementContexts);
        
        var isDynamic = dynamic || dataType == null;
        
        if (map.All(element => element.Value is IConstantValue))
        {
            var value = map.ToDictionary(
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

        foreach (var kvp in map)
        {
            if (kvp.Value is IConstantValue constantValue)
            {
                parts.Add(kvp.Key, constantValue);
            }
            if (kvp.Value is IRuntimeValue runtimeValue)
            {
                parts.Add(kvp.Key, runtimeValue.AsConstantSubstitute);

                substitutions.Add(new KeyValuePair<object, IRuntimeValue>(kvp.Key, runtimeValue));
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
        
        void ProcessObjectElements(IEnumerable<AmethystParser.ObjectElementContext> objectElementContexts)
        {
            foreach (var objectElementContext in objectElementContexts)
            {
                if (objectElementContext.preprocessorYieldingStatement() is { } preprocessorYieldingStatementContext)
                {
                    var result = VisitPreprocessorYieldingStatement<AmethystParser.ObjectElementContext>(preprocessorYieldingStatementContext);
                    ProcessObjectElements(result);
                }
                else if (objectElementContext.objectKvp() is { } objectKvpContext)
                {
                    var key = objectKvpContext.IDENTIFIER().Symbol.Text;
                    var result = VisitExpression(objectKvpContext.expression());
            
                    // if the initial data type differs from the current data type, we need to make the array dynamic
                    if (!dynamic && dataType != null && dataType != result.Datatype)
                    {
                        dynamic = true;
                    }

                    // set the initial data type if it is not set yet
                    if (!dynamic)
                    {
                        dataType ??= result.Datatype;
                    }
            
                    map.Add(key, result);
                }
            }
        }
    }
}