namespace Amethyst;

public abstract class ArrayBase : RuntimeValue, IIndexable
{
    public override BooleanResult MakeBoolean()
    {
        var location = ++Compiler.StackPointer;
        
        Compiler.AddCode($"execute store success score {location} amethyst run data get storage amethyst: {Location}[0]");
        
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
        
        Compiler.AddCode($"execute store result score {location} amethyst run data get storage amethyst: {Location}");
        
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
        
        if (index is IntegerConstant integerConstant)
        {
            AddCode($"execute store result storage amethyst: {location} run data get storage amethyst: {Location}[{integerConstant.Value}]");
        }
        
        else if (index is IntegerResult integerResult)
        {
            var indexLocation = integerResult.NextFreeLocation();
            Compiler.StackPointer--;
            
            AddCode($"execute store result storage amethyst: {indexLocation} run scoreboard players get {integerResult.Location} amethyst");
            
            var scope = Compiler.EvaluateScoped("_index", _ =>
            {
                AddCode($"$execute store result storage amethyst: {location} run data get storage amethyst: {Location}[$({integerResult.Location})]");
            });
            
            AddCode($"function {scope.McFunctionPath} with storage amethyst:");
        }
        
        else
        {
            throw new SyntaxException("Expected integer index.", index.Context);
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