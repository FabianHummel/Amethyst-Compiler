using Amethyst.Language;

namespace Amethyst;

public partial class Compiler
{
    public override object? VisitDebug_statement(AmethystParser.Debug_statementContext context)
    {
        var result = VisitExpression(context.expression());
        
        
        
        Scope.AddCode($$"""
                        tellraw @a ["",{"text":"[DEBUG]: ","bold":true,"italic":true,"color":"gray"},{"nbt":"_out","storage":"amethyst","interpret":true}]
                        """);

        return null;
    }
}