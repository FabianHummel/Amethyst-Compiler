using Amethyst.Model;

namespace Amethyst;

public class StringResult : RuntimeValue
{
    public override DataType DataType => new()
    {
        BasicType = BasicType.String,
        Modifier = null
    };

    public override BooleanResult MakeBoolean()
    {
        Compiler.AddCode($"execute store success score {++Compiler.StackPointer} amethyst run data get storage amethyst: {Location}");
        return new BooleanResult
        {
            Location = Compiler.StackPointer.ToString(),
            Compiler = Compiler,
            Context = Context
        };
    }

    public override IntegerResult MakeInteger()
    {
        Compiler.AddCode($"execute store result score {++Compiler.StackPointer} amethyst run data get storage amethyst: {Location}");
        return new IntegerResult
        {
            Location = Compiler.StackPointer.ToString(),
            Compiler = Compiler,
            Context = Context
        };
    }

    protected override RuntimeValue VisitAdd(StringResult rhs)
    {
        var lhsLocation = Location;
        
        if (!IsTemporary)
        {
            lhsLocation = (++Compiler.StackPointer).ToString();
        }
        
        var scope = Compiler.EvaluateScoped("_concat", _ =>
        {
            // Todo: sanitize string by escaping quotes and other special characters that may mess up the macro expansion
            Compiler.AddCode($"$data modify storage amethyst: {lhsLocation} set value \"$({Location})$({rhs.Location})\"");
        });
        
        Compiler.AddCode($"function {scope.McFunctionPath} with storage amethyst:");

        return new StringResult
        {
            Compiler = Compiler,
            Context = Context,
            Location = lhsLocation,
            IsTemporary = true
        };
    }
}