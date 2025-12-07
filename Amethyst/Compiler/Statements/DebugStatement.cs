using Amethyst.Language;
using Amethyst.Model;

namespace Amethyst;

public partial class Compiler
{
    /// <inheritdoc />
    /// <summary>
    ///     <p>Prints out a value to the server console when this statement is run. This works with any
    ///     sort of values, whether it being a constant or only known during runtime. The value is
    ///     formatted with syntax highlighting to match the format of any <c>/data get</c> command.</p>
    ///     <p>This works by calling the internal Amethyst API that recursively traverses the data
    ///     structure and assembles a formatted string for each visited value.</p>
    ///     <p><inheritdoc /></p>
    /// </summary>
    /// <example><c>debug "Hello World!"</c> → <c>[server] Hello World!</c></example>
    /// <seealso cref="VisitPreprocessorDebugStatement" />
    public override object? VisitDebugStatement(AmethystParser.DebugStatementContext context)
    {
        var result = VisitExpression(context.expression());

        string jsonComponent = null!;
        
        if (result is IConstantValue constantValue)
        {
            jsonComponent = constantValue.ToTextComponent();
        }

        if (result is IRuntimeValue runtimeValue)
        {
            if (runtimeValue.Datatype.IsScoreboardType(out var scoreboardDatatype))
            {
                this.AddCode($"execute store result storage amethyst:internal data.stringify.in {scoreboardDatatype.StorageModifier} run scoreboard players get {runtimeValue.Location}");
            }
            else
            {
                this.AddCode($"data modify storage amethyst:internal data.stringify.in set from storage {runtimeValue.Location}");
            }
            this.AddCode("function amethyst:internal/data/stringify/run");
            
            jsonComponent = """{"nbt":"data.stringify.out","storage":"amethyst:internal","interpret":true}""";
        }
        
        this.AddCode($$"""tellraw @a ["",{"text":"DEBUG: ","color":"dark_gray"},{{jsonComponent}}]""");
        
        return null;
    }
}