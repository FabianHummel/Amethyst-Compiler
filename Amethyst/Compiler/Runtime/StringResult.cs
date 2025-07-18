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
        var location = ++Compiler.StackPointer;
        
        Compiler.AddCode($"execute store success score {location} amethyst run data get storage amethyst: {Location}");
        
        return new BooleanResult
        {
            Location = Compiler.StackPointer,
            Compiler = Compiler,
            Context = Context
        };
    }

    public override IntegerResult MakeInteger()
    {
        var location = ++Compiler.StackPointer;
        
        Compiler.AddCode($"execute store result score {location} amethyst run data get storage amethyst: {Location}");
        
        return new IntegerResult
        {
            Location = location,
            Compiler = Compiler,
            Context = Context
        };
    }

    protected override RuntimeValue VisitAdd(StringResult rhs)
    {
        var resultLocation = Location;

        if (rhs.IsTemporary)
        {
            resultLocation = rhs.Location;
        }
        else if (!IsTemporary)
        {
            resultLocation = ++Compiler.StackPointer;
        }
        
        var scope = Compiler.EvaluateScoped("_concat", _ =>
        {
            // Todo: sanitize string by escaping quotes and other special characters that may mess up the macro expansion
            Compiler.AddCode($"$data modify storage amethyst: {resultLocation} set value \"$({Location})$({rhs.Location})\"");
        });
        
        Compiler.AddCode($"function {scope.McFunctionPath} with storage amethyst:");

        return new StringResult
        {
            Compiler = Compiler,
            Context = Context,
            Location = resultLocation,
            IsTemporary = true
        };
    }
}