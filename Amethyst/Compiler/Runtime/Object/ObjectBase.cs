namespace Amethyst;

public abstract class ObjectBase : RuntimeValue, IIndexable
{
    public override BooleanResult MakeBoolean()
    {
        var location = ++Compiler.StackPointer;
        
        Compiler.AddCode($"execute store success score {location} amethyst run data get storage amethyst: {Location}.keys[0]");
        
        return new BooleanResult
        {
            Location = location,
            Compiler = Compiler,
            Context = Context,
            IsTemporary = true
        };
    }

    public override IntegerResult MakeInteger()
    {
        var location = ++Compiler.StackPointer;
        
        Compiler.AddCode($"execute store result score {location} amethyst run data get storage amethyst: {Location}.keys");
        
        return new IntegerResult
        {
            Location = location,
            Compiler = Compiler,
            Context = Context,
            IsTemporary = true
        };
    }

    public AbstractResult GetIndex(AbstractResult index)
    {
        var location = NextFreeLocation();
        
        if (index is StringConstant stringConstant)
        {
            AddCode($"execute store result storage amethyst: {location} run data get storage amethyst: {Location}.data.{stringConstant.Value}");
        }
        
        else if (index is StringResult stringResult)
        {
            var scope = Compiler.EvaluateScoped("_index", _ =>
            {
                AddCode($"$execute store result storage amethyst: {location} run data get storage amethyst: {Location}.data.$({stringResult.Location})");
            });
            
            AddCode($"function {scope.McFunctionPath} with storage amethyst:");
        }
        
        else
        {
            throw new SyntaxException("Expected string index.", index.Context);
        }

        return new UnknownResult
        {
            Compiler = Compiler,
            Context = Context,
            Location = location,
            IsTemporary = true
        };
    }
}