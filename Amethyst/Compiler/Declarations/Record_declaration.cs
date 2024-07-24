using System.Diagnostics;
using Amethyst.Language;
using Amethyst.Model;
using Type = Amethyst.Model.Type;

namespace Amethyst;

public partial class Compiler
{
    private string GetRecordName()
    {
        return $"amethyst_record_{TotalRecordCount-1}";
    }
    
    public override object? VisitRecord_declaration(AmethystParser.Record_declarationContext context)
    {
        TotalRecordCount++;
        
        var recordName = context.identifier().GetText();
        
        if (Scope.Records.ContainsKey(recordName))
        {
            throw new SyntaxException($"The record '{recordName}' has already been declared.", context);
        }
        
        var name = GetRecordName();
        
        Result? result = null;
        Type? type = null;
        
        if (context.expression() is { } expression)
        {
            result = VisitExpression(expression);
        }
        
        // if a type is defined, set the type to the defined type
        if (context.type() is { } typeContext)
        {
            type = VisitType(typeContext);
        }
        // if both types are defined, check if they match
        if (type != null && result != null && type != result.Type)
        {
            throw new SyntaxException($"The type of the record '{type}' does not match the inferred type '{result.Type}'.", context);
        }
        // if no type is defined or inferred, throw an error
        if (type == null && result == null)
        {
            throw new SyntaxException("Record declarations must have specified a type or an expression to infer the type from.", context);
        }
        // if no type is defined, but inferred, set the type to the inferred type
        if (type == null && result != null)
        {
            type = result.Type;
        }
        
        Debug.Assert(type != null, nameof(type) + " != null");
        
        var attributes = VisitAttribute_list(context.attribute_list());

        Scope.Records.Add(recordName, new Record
        {
            Name = name,
            Type = type,
            InitialValue = result,
            Attributes = attributes
        });

        if (type.IsScoreboardType)
        {
            AddInitCode($"scoreboard objectives add {name} dummy");

            if (Program.DebugMode)
            {
                AddInitCode($$"""scoreboard objectives modify {{name}} displayname ["",{"text":"Record ","bold":true},{"text":"{{name}}","color":"dark_purple"},{"text":" @ "},{"text":"{{Scope.McFunctionPath}}/","color":"gray"},{"text":"{{recordName}}","color":"light_purple"}]""");
            }
        }

        if (result != null)
        {
            if (result.Type.IsScoreboardType)
            {
                AddCode($"scoreboard players operation {name} amethyst_record_initializers = {result.Location} amethyst");
            }
            else if (result.Type.IsStorageType)
            {
                AddCode($"data modify storage amethyst:record_initializers {name} set from storage amethyst:stack {result.Location}");
            }
        }

        return null;
    }
}