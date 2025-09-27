namespace Amethyst;

public abstract partial class AbstractRuntimeObject : AbstractObject, IRuntimeValue, IIndexable
{
    public int Location { get; set; }
    
    public bool IsTemporary { get; set; }

    public AbstractBoolean MakeBoolean()
    {
        var location = ++Compiler.StackPointer;
        
        Compiler.AddCode($"execute store success score {location} amethyst run data get storage amethyst: {Location}.keys[0]");
        
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
        
        Compiler.AddCode($"execute store result score {location} amethyst run data get storage amethyst: {Location}.keys");
        
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
        
        if (index is ConstantString stringConstant)
        {
            AddCode($"execute store result storage amethyst: {location} run data get storage amethyst: {Location}.data.{stringConstant.Value}");
        }
        
        else if (index is RuntimeString stringResult)
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

        return new RuntimeUnknown
        {
            Compiler = Compiler,
            Context = Context,
            Location = location,
            IsTemporary = true
        };
    }
}