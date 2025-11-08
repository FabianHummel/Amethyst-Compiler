using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    /// <summary>
    ///     <p>A variable declaration. A variable holds a single value and can be stored in either a
    ///     scoreboard or storage, depending on the datatype.</p>
    ///     <p><inheritdoc /></p></summary>
    /// <exception cref="SymbolAlreadyDeclaredException">A symbol with the same name already exists in the
    /// current scope.</exception>
    public override Symbol VisitVariableDeclaration(AmethystParser.VariableDeclarationContext context)
    {
        var variableName = context.IDENTIFIER().GetText();
        if (EnsureSymbolIsNewOrGetRootSymbol(variableName, context, out var symbol))
        {
            return symbol;
        }
        
        var result = VisitExpression(context.expression()).EnsureRuntimeValue();
        var type = GetOrInferTypeResult(result, context.type(), context);
        var attributes = VisitAttributeList(context.attributeList());

        var variable = new Variable
        {
            Name = variableName,
            Location = result.Location,
            Datatype = type,
            Attributes = attributes
        };
        
        Scope.Symbols.Add(variableName, variable);
        
        return variable;
    }
}