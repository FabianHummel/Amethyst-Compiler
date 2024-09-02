using Amethyst.Model;

namespace Amethyst;

public class StringResult : AbstractResult
{
    public override DataType DataType => new()
    {
        BasicType = BasicType.String,
        Modifier = null
    };

    public override BooleanResult MakeBoolean()
    {
        Compiler.AddCode($"execute store success score {++Compiler.MemoryLocation} amethyst run data get storage amethyst: {Location}");
        return new BooleanResult
        {
            Location = Compiler.MemoryLocation.ToString(),
            Compiler = Compiler,
            Context = Context
        };
    }

    public override IntegerResult MakeNumber()
    {
        Compiler.AddCode($"execute store result score {++Compiler.MemoryLocation} amethyst run data get storage amethyst: {Location}");
        return new IntegerResult
        {
            Location = Compiler.MemoryLocation.ToString(),
            Compiler = Compiler,
            Context = Context
        };
    }

    protected override AbstractResult VisitAdd(StringResult rhs)
    {
        var previousLocation = Location;
        if (!IsTemporary)
        {
            previousLocation = MemoryLocation++.ToString();
        }
        
        var scope = Compiler.EvaluateScoped("_concat", () =>
        {
            // Todo: sanitize string by escaping quotes and other special characters that may mess up the macro expansion
            Compiler.AddCode($"$data modify storage amethyst: {previousLocation} set value \"$({Location})$({rhs.Location})\"");
        });
        
        Compiler.AddCode($"function {scope.McFunctionPath} with storage amethyst:");

        return new StringResult
        {
            Compiler = Compiler,
            Context = Context,
            Location = previousLocation,
            IsTemporary = true
        };
    }

    public override AbstractResult MakeVariable()
    {
        if (Location != null)
        {
            return this;
        }
        
        AddCode($"data modify storage amethyst: {MemoryLocation} set value {ConstantValue}");
        
        return new StringResult
        {
            Compiler = Compiler,
            Context = Context,
            Location = MemoryLocation++.ToString(),
            IsTemporary = true,
        };
    }
}