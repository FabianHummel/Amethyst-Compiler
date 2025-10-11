using Amethyst.Language;
using Amethyst.Model;
using Antlr4.Runtime;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitCallExpression(AmethystParser.CallExpressionContext context)
    {
        var functionName = context.IDENTIFIER().GetText();

        if (!TryGetSymbol(functionName, out var functionSymbol, context) || functionSymbol is not Function function)
        {
            throw new SyntaxException($"Function '{functionName}' is not defined.", context);
        }

        AmethystParser.ExpressionContext[]? expressionContexts = null;
        if (context.argumentList() is { } argumentListContext)
        {
            expressionContexts = argumentListContext.expression();
        }
        
        VisitCallExpressionInternal(function, expressionContexts, context);

        return null;
    }

    private void VisitCallExpressionInternal(Function function, AmethystParser.ExpressionContext[]? expressionContexts, ParserRuleContext context)
    {
        if (expressionContexts != null)
        {
            VisitArgumentListInternal(function, expressionContexts, context);
        }
        
        AddCode($"function {function.Scope.McFunctionPath}");
    }
    
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
                AddCode($"scoreboard players set {parameter.Location} {scoreboardValue.ScoreboardValue}");
                continue;
            }

            if (parameter.Location.DataLocation == DataLocation.Scoreboard && result is IRuntimeValue runtimeValue)
            {
                AddCode($"scoreboard players operation {parameter.Location} = {runtimeValue.Location} amethyst");
                continue;
            }

            if (parameter.Location.DataLocation == DataLocation.Storage && result is IConstantValue storageValue)
            {
                AddCode($"data modify storage {parameter.Location} set value {storageValue.ToNbtString()}");
                continue;
            }
            
            if (parameter.Location.DataLocation == DataLocation.Storage && result is IRuntimeValue runtimeStorageValue)
            {
                AddCode($"data modify storage {parameter.Location} set from storage {runtimeStorageValue.Location}");
                continue;
            }

            throw new SyntaxException($"Cannot pass value of type '{result.Datatype}' to parameter '{parameter.Name}' of type '{parameter.Datatype}'.", expressionContext);
        }
    }
}