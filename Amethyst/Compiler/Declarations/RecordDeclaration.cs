using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    /// <inheritdoc />
    /// <summary>
    ///     <p>A record declaration. A record is a set of values tied to individual entities in the game.
    ///     This rather unconventional data structure allows for per-entity data and bridges the gap
    ///     between Minecraft scoreboards and complex storage such as decimal numbers or any NBT data.</p>
    ///     <p>The expression's result of assigning a record is a value that is set upon a specified event,
    ///     such as when a player joins the game or triggers an advancement. Note that the initial value is
    ///     only evaluated once when the record is declared rather than every time the event is emitted.
    ///     The initilialization value is stored in either a scoreboard or storage, depending on the
    ///     datatype.</p>
    ///     <p><inheritdoc /></p></summary>
    /// <exception cref="SymbolAlreadyDeclaredException">A symbol with the same name already exists in the
    /// current scope.</exception>
    public override Symbol VisitRecordDeclaration(AmethystParser.RecordDeclarationContext context)
    {
        var recordName = context.IDENTIFIER().GetText();
        if (EnsureSymbolIsNewOrGetRootSymbol(recordName, context, out var symbol))
        {
            return symbol;
        }
        
        var result = VisitExpression(context.expression()).EnsureRuntimeValue();
        var type = GetOrInferTypeResult(result, context.type(), context);
        var attributes = VisitAttributeList(context.attributeList());
        var name = $"amethyst_record_{result.Location}";

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

        return record;
    }
}