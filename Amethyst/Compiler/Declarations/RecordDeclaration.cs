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

        var record = new Record
        {
            Name = name,
            Datatype = type,
            InitialValue = result,
            Attributes = attributes,
            McFunctionPath = Scope.McFunctionPath
        };
        
        Scope.Symbols.Add(recordName, record);

        if (type.DataLocation == DataLocation.Scoreboard)
        {
            Namespace.RecordDeclarations.Add(recordName, record);
        }

        if (result.Location.DataLocation == DataLocation.Scoreboard)
        {
            this.AddCode($"scoreboard players operation {name} amethyst_record_initializers = {result.Location}");
        }
        else
        {
            this.AddCode($"data modify storage amethyst:record_initializers {name} set from storage {result.Location}");
        }

        return null;
    }
}