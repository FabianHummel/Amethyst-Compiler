using Amethyst.Language;
using Amethyst.Model;
using Antlr4.Runtime;

namespace Amethyst;

public partial class Compiler
{
    /// <inheritdoc />
    /// <summary>
    ///     <p>The textual representation of types is converted to its enum equivalent by comparing the
    ///     enum's description attribute. Additionally, a modifier is also parsed that indicates whether
    ///     the type should be interpreted as an array or object of the original type. Decimals are handled
    ///     specially by also parsing the number of decimal places if explicitly defined, otherwise, the
    ///     <see cref="DecimalDatatype.DEFAULT_DECIMAL_PLACES">default number of decimal places</see> is
    ///     used instead.</p>
    ///     <p><inheritdoc /></p></summary>
    /// <example><c>int</c> -> <see cref="BasicType.Int" /> without any modifier<br /> <c>bool[]</c> ->
    /// <see cref="BasicType.Bool" /> with an <see cref="Modifier.Array" /> modifier<br /><c>dec(3){}</c>
    /// -> <see cref="BasicType.Dec" /> with 3 decimal places and an <see cref="Modifier.Object" />
    /// modifier</example>
    /// <exception cref="InvalidOperationException">The type is unknown.</exception>
    public override AbstractDatatype VisitType(AmethystParser.TypeContext context)
    {
        Modifier? modifier = null;
        if (context.LRBRACKET() != null)
        {
            modifier = Modifier.Array;
        }
        else if (context.LRBRACE() != null)
        {
            modifier = Modifier.Object;
        }
        else if (context.modifier is { } modifierContext)
        {
            throw new InvalidOperationException($"Invalid type modifier '{modifierContext.Text}'.");
        }
        
        if (context.@decimal() is { } decimalContext)
        {
            var numDecimalPlaces = DecimalDatatype.DEFAULT_DECIMAL_PLACES;

            if (decimalContext.INTEGER_LITERAL() is { } integerLiteral)
            {
                numDecimalPlaces = int.Parse(integerLiteral.Symbol.Text);
            }
            
            return new DecimalDatatype(numDecimalPlaces)
            {
                Modifier = modifier
            };
        }

        if (context.rawLocation() is { } rawLocationContext)
        {
            return VisitRawLocation(rawLocationContext);
        }

        BasicType basicType;
        if (context.INT() != null)
        {
            basicType = BasicType.Int;
        }
        else if (context.STRING() != null)
        {
            basicType = BasicType.String;
        }
        else if (context.BOOL() != null)
        {
            basicType = BasicType.Bool;
        }
        else if (context.ARRAY() != null)
        {
            basicType = BasicType.Array;
        }
        else if (context.OBJECT() != null)
        {
            basicType = BasicType.Object;
        }
        else if (context.ENTITY() != null)
        {
            basicType = BasicType.Entity;
        }
        else
        {
            var basicTypeString = context.GetChild(0).GetText();
            throw new InvalidOperationException($"Invalid basic type '{basicTypeString}'.");
        }
        
        return AbstractDatatype.Parse(basicType, modifier);
    }

    /// <summary>Evaluates a value's type by either retrieving it directly or inferring it if it's not
    /// explicitly defined.</summary>
    /// <param name="value">The value to get the type from.</param>
    /// <param name="typeContext">Optionally an explicit type declaration. If this is defined, it is used
    /// as the resulting type.</param>
    /// <param name="context">The parser rule context used for error handling.</param>
    /// <returns>The datatype of the specified <paramref name="value" />.</returns>
    /// <exception cref="SyntaxException">The inferred type of <paramref name="value" /> does not match the
    /// explicitly declared type of <paramref name="typeContext" />.</exception>
    private AbstractDatatype GetOrInferTypeResult(IRuntimeValue value, AmethystParser.TypeContext? typeContext, ParserRuleContext context)
    {
        AbstractDatatype? type = null;
        
        if (typeContext != null)
        {
            type = VisitType(typeContext);
        }
        // if two types are defined, check if they match
        if (type != null && type != value.Datatype)
        {
            throw new SyntaxException($"The type '{type}' does not match the inferred type '{value.Datatype}'.", context);
        }
        // if no type is defined, we infer it
        if (type == null)
        {
            type = value.Datatype;
        }
        
        return type;
    }
}