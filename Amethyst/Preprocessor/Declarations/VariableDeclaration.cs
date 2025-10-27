using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    /// <summary>Creates a preprocessor variable declaration in the <see cref="Scope">current scope</see>.
    /// <br /><inheritdoc /></summary>
    /// <exception cref="SemanticException">The symbol has already been declared in this or an enclosing
    /// scope.</exception>
    /// <exception cref="SyntaxException">The type of the variable is invalid or cannot be inferred.</exception>
    public override object? VisitPreprocessorVariableDeclaration(AmethystParser.PreprocessorVariableDeclarationContext context)
    {
        var variableName = context.IDENTIFIER().GetText();
        
        if (TryGetSymbol(variableName, out _, context))
        {
            throw new SemanticException($"The symbol '{variableName}' has already been declared.", context);
        }

        var result = VisitPreprocessorExpression(context.preprocessorExpression());
        
        PreprocessorDatatype? type = null;
        
        // if a type is defined, set the type to the defined type
        if (context.preprocessorType() is { } preprocessorTypeContext)
        {
            type = VisitPreprocessorType(preprocessorTypeContext);
        }
        // if both types are defined, check if they match
        if (type != null && result != null && type != result.Datatype)
        {
            throw new SyntaxException($"The type of the variable '{type}' does not match the inferred type '{result.Datatype}'.", context);
        }
        // if no type is defined or inferred, throw an error
        if (type == null && result == null)
        {
            throw new SyntaxException("Variable declarations must have specified a type or an expression to infer the type from.", context);
        }
        // if no type is defined, but inferred, set the type to the inferred type
        if (type == null && result != null)
        {
            type = result.Datatype;
        }
        
        Scope.Symbols.Add(variableName, new PreprocessorVariable
        {
            Datatype = type!,
            Value = result!
        });
        
        return null;
    }
}