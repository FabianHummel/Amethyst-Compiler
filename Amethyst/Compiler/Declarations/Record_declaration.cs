using Amethyst.Language;
using Amethyst.Model;
using Type = Amethyst.Model.Type;

namespace Amethyst;

public partial class Compiler
{
    private string GetRecordName()
    {
        return $"amethyst_r{Scope.RecordCount}";
    }
    
    public override object? VisitRecord_declaration(AmethystParser.Record_declarationContext context)
    {
        var recordName = context.identifier().GetText();
        
        var name = GetRecordName();
        
        object? result = null; // Todo: This will be of type 'Location'
        Type? type = null;
        
        if (context.expression() is { } expression)
        {
            result = VisitExpression(expression); // Todo: Destination targeting (tell VisitExpression that the result should be stored in 'name')
        }
        
        if (context.type() is { } typeContext)
        {
            type = VisitType(typeContext);
        }
        else if (result == null)
        {
            ThrowSyntaxError("Record declarations must have specified a type or an expression to infer the type from.", context); 
        }
        
        if (/*InferType(result, out var inferredType)*/ false) // Todo: result.Type
        {
            // if (type is not null && !type.Equals(inferredType))
            // {
            //     ThrowSyntaxError($"The type of the record '{type}' does not match the inferred type '{inferredType}'.", context);
            // }
            //
            // type = inferredType;
        }
        else if (type is null)
        {
            ThrowSyntaxError("Could not infer the type of the record.", context);
        }
        
        if (Scope.Records.ContainsKey(recordName))
        {
            ThrowSyntaxError($"The record '{recordName}' has already been declared.", context);
        }

        Scope.Records.Add(recordName, new Record
        {
            Name = name,
            Type = type!,
            Attributes = context.attribute_list().SelectMany(Attribute_listContext =>
            {
                return Attribute_listContext.attribute().Select(Attribute_listContext_inner =>
                {
                    return Attribute_listContext_inner.identifier().GetText();
                });
            }).ToList()
        });

        if (type!.IsScoreboardType)
        {
            AddInitCode($"scoreboard objectives add {name} dummy");
            AddInitCode($"scoreboard players reset * {name}");

            if (Program.DebugMode)
            {
                AddInitCode($$"""scoreboard objectives modify {{name}} displayname ["",{"text":"Record ","bold":true},{"text":"{{name}}","color":"dark_purple"},{"text":" @ "},{"text":"{{Scope.McFunctionPath}}/","color":"gray"},{"text":"{{recordName}}","color":"light_purple"}]""");
            }
        }
        else
        {
            AddInitCode($"data modify storage amethyst:records {name} set value");
        }

        return null;
    }
}