using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitPreprocessorVariableDeclaration(AmethystParser.PreprocessorVariableDeclarationContext context)
    {
        if (context.IDENTIFIER() is not { } variableNameContext)
        {
            throw new SyntaxException("Expected variable name.", context);
        }
        
        var variableName = variableNameContext.GetText();
        
        if (Scope.TryGetSymbol(variableName, out _))
        {
            throw new SyntaxException($"The symbol '{variableName}' has already been declared.", context);
        }
        
        if (context.preprocessorExpression() is not { } preprocessorExpressionContext)
        {
            throw new SyntaxException("Expected preprocessor expression.", context);
        }

        var result = VisitPreprocessorExpression(preprocessorExpressionContext);
        
        PreprocessorDataType? type = null;
        
        // if a type is defined, set the type to the defined type
        if (context.preprocessorType() is { } preprocessorTypeContext)
        {
            type = VisitPreprocessorType(preprocessorTypeContext);
        }
        // if both types are defined, check if they match
        if (type != null && result != null && type != result.DataType)
        {
            throw new SyntaxException($"The type of the variable '{type}' does not match the inferred type '{result.DataType}'.", context);
        }
        // if no type is defined or inferred, throw an error
        if (type == null && result == null)
        {
            throw new SyntaxException("Variable declarations must have specified a type or an expression to infer the type from.", context);
        }
        // if no type is defined, but inferred, set the type to the inferred type
        if (type == null && result != null)
        {
            type = result.DataType;
        }
        
        Scope.Symbols.Add(variableName, new PreprocessorVariable
        {
            DataType = type!,
            Value = result!
        });
        
        return null;
    }
}