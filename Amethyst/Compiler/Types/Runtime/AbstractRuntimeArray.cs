namespace Amethyst;

public abstract partial class AbstractRuntimeArray : AbstractArray, IRuntimeValue, IIndexable
{
    public int Location { get; set; }
    
    public bool IsTemporary { get; set; }

    public AbstractBoolean MakeBoolean()
    {
        var location = ++Compiler.StackPointer;
        
        Compiler.AddCode($"execute store success score {location} amethyst run data get storage amethyst: {Location}[0]");
        
        return new RuntimeBoolean
        {
            Location = location,
            Compiler = Compiler,
            Context = Context,
            IsTemporary = true
        };
    }

    public AbstractInteger MakeInteger()
    {
        var location = ++Compiler.StackPointer;
        
        Compiler.AddCode($"execute store result score {location} amethyst run data get storage amethyst: {Location}");
        
        return new RuntimeInteger
        {
            Location = location,
            Compiler = Compiler,
            Context = Context,
            IsTemporary = true
        };
    }

    public AbstractValue GetIndex(AbstractValue index)
    {
        var location = NextFreeLocation();
        
        if (index is ConstantInteger integerConstant)
        {
            AddCode($"execute store result storage amethyst: {location} run data get storage amethyst: {Location}[{integerConstant.Value}]");
        }
        else if (index is RuntimeInteger and IRuntimeValue integerResult)
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

        return new RuntimeUnknown
        {
            Compiler = Compiler,
            Context = Context,
            Location = location,
            IsTemporary = true
        };
    }
}