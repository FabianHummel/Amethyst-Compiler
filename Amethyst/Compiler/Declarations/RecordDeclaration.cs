using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    private string GetRecordName()
    {
        return $"amethyst_record_{TotalRecordCount-1}";
    }
    
    public override object? VisitRecordDeclaration(AmethystParser.RecordDeclarationContext context)
    {
        TotalRecordCount++;
        
        var recordName = context.IDENTIFIER().GetText();
        if (TryGetSymbol(recordName, out _, context))
        {
            throw new SymbolAlreadyDeclaredException(recordName, context);
        }
        
        var name = GetRecordName();
        
        var result = VisitExpression(context.expression()).EnsureRuntimeValue();
        
        var type = GetOrInferTypeResult(result, context.type(), context);
        
        var attributes = VisitAttributeList(context.attributeList());

        Scope.Symbols.Add(recordName, new Record
        {
            Name = name,
            DataType = type,
            InitialValue = result,
            Attributes = attributes
        });

        if (type.Location == DataLocation.Scoreboard)
        {
            AddInitCode($"scoreboard objectives add {name} dummy");

            if (Context.CompilerFlags.HasFlag(CompilerFlags.Debug))
            {
                AddInitCode($$"""scoreboard objectives modify {{name}} displayname ["",{"text":"Record ","bold":true},{"text":"{{name}}","color":"dark_purple"},{"text":" @ "},{"text":"{{Scope.McFunctionPath}}/","color":"gray"},{"text":"{{recordName}}","color":"light_purple"}]""");
            }
        }

        if (result.DataType.Location == DataLocation.Scoreboard)
        {
            AddCode($"scoreboard players operation {name} amethyst_record_initializers = {result.Location} amethyst");
        }
        else
        {
            AddCode($"data modify storage amethyst:record_initializers {name} set from storage amethyst:stack {result.Location}");
        }

        return null;
    }
}