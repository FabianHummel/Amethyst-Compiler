using Amethyst.Language;
using Amethyst.Model;
using Antlr4.Runtime;

namespace Amethyst;

public partial class Compiler
{
    /// <inheritdoc />
    /// <summary>
    ///     <p>Calls a function and handles parameters and the return value. Arguments passed to the
    ///     function are set to the correct location before calling the actual function</p>
    ///     <p><inheritdoc /></p></summary>
    /// <exception cref="SemanticException">The symbol is not a function.</exception>
    public override object? VisitCallExpression(AmethystParser.CallExpressionContext context)
    {
        var functionName = context.IDENTIFIER().GetText();
        
        if (GetSymbol(functionName, context) is not Function function)
        {
            throw new SemanticException($"The symbol '{functionName}' is not a function and cannot be called.", context);
        }

        AmethystParser.ExpressionContext[]? expressionContexts = null;
        if (context.argumentList() is { } argumentListContext)
        {
            expressionContexts = argumentListContext.expression();
        }
        
        if (expressionContexts != null)
        {
            VisitArgumentListInternal(function, expressionContexts, context);
        }
        
        this.AddCode($"function {function.McFunctionPath}");

        return null;
    }

    /// <summary>Processes the arguments passed to the function. This method allocates the expressions to
    /// the correct locations and converts between storages and scoreboards.</summary>
    /// <param name="function">The function that is being called. Used to resolve its parameters.</param>
    /// <param name="expressionContexts">The list of expressions passed to the function.</param>
    /// <param name="context">The parser rule context used for error handling.</param>
    /// <exception cref="SyntaxException">The number of arguments does not match the required amount.</exception>
    /// <exception cref="SyntaxException">An argument's type does not match the corresponding parameter's
    /// type.</exception>
    private void VisitArgumentListInternal(Function function, AmethystParser.ExpressionContext[] expressionContexts, ParserRuleContext context)
    {
        if (expressionContexts.Length != function.Parameters.Length)
        {
            throw new SyntaxException($"Expected {function.Parameters.Length} arguments, but got {expressionContexts.Length}.", context);
        }
        
        for (var index = 0; index < expressionContexts.Length; index++)
        {
            var expressionContext = expressionContexts[index];
            var parameter = function.Parameters[index];
            var result = VisitExpression(expressionContext);
            
            if (result.Datatype != parameter.Datatype)
            {
                throw new SyntaxException($"Expected type '{parameter.Datatype}', but got '{result.Datatype}'.", expressionContext);
            }
            
            // TODO: Find a way to make this better
            // TODO: Handle more cases: from scoreboard -> storage, from storage -> scoreboard

            if (parameter.Location.DataLocation == DataLocation.Scoreboard && result is IScoreboardValue scoreboardValue)
            {
                this.AddCode($"scoreboard players set {parameter.Location} {scoreboardValue.ScoreboardValue}");
                continue;
            }

            if (parameter.Location.DataLocation == DataLocation.Scoreboard && result is IRuntimeValue runtimeValue)
            {
                this.AddCode($"scoreboard players operation {parameter.Location} = {runtimeValue.Location} amethyst");
                continue;
            }

            if (parameter.Location.DataLocation == DataLocation.Storage && result is IConstantValue storageValue)
            {
                this.AddCode($"data modify storage {parameter.Location} set value {storageValue.ToNbtString()}");
                continue;
            }
            
            if (parameter.Location.DataLocation == DataLocation.Storage && result is IRuntimeValue runtimeStorageValue)
            {
                this.AddCode($"data modify storage {parameter.Location} set from storage {runtimeStorageValue.Location}");
                continue;
            }

            throw new SyntaxException($"Cannot pass value of type '{result.Datatype}' to parameter '{parameter.Name}' of type '{parameter.Datatype}'.", expressionContext);
        }
    }
}