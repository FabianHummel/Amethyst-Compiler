using Amethyst.Language;
using Amethyst.Model;

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

        if (context.argumentList() is { } argumentListContext)
        {
            VisitArgumentList(argumentListContext, function);
        }
        
        AddCode($"function {function.Scope.McFunctionPath}");

        return null;
    }
    
    public void VisitArgumentList(AmethystParser.ArgumentListContext context, Function function)
    {
        var expressionContexts = context.expression();
        
        if (expressionContexts.Length != function.Parameters.Length)
        {
            throw new SyntaxException($"Expected {function.Parameters.Length} arguments, but got {expressionContexts.Length}.", context);
        }
        
        for (var index = 0; index < expressionContexts.Length; index++)
        {
            var expressionContext = expressionContexts[index];
            var parameter = function.Parameters[index];
            var result = VisitExpression(expressionContext);
            
            if (result.DataType != parameter.DataType)
            {
                throw new SyntaxException($"Expected type '{parameter.DataType}', but got '{result.DataType}'.", expressionContext);
            }

            if (parameter.DataType.Location == DataLocation.Scoreboard && result is IScoreboardValue scoreboardValue)
            {
                AddCode($"scoreboard players set {parameter.Location} amethyst {scoreboardValue.ScoreboardValue}");
                continue;
            }

            if (parameter.DataType.Location == DataLocation.Scoreboard && result is IRuntimeValue runtimeValue)
            {
                AddCode($"scoreboard players operation {parameter.Location} amethyst = {runtimeValue.Location} amethyst");
                continue;
            }

            if (parameter.DataType.Location == DataLocation.Storage && result is IConstantValue storageValue)
            {
                AddCode($"data modify storage amethyst: {parameter.Location} set value {storageValue.ToNbtString()}");
                continue;
            }
            
            if (parameter.DataType.Location == DataLocation.Storage && result is IRuntimeValue runtimeStorageValue)
            {
                AddCode($"data modify storage amethyst: {parameter.Location} set from storage amethyst: {runtimeStorageValue.Location}");
                continue;
            }

            throw new SyntaxException($"Cannot pass value of type '{result.DataType}' to parameter '{parameter.Name}' of type '{parameter.DataType}'.", expressionContext);
        }
    }
}