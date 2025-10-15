using Amethyst.Utility;

namespace Amethyst.Model;

public sealed class Registry
{
    public Registry(string name)
    {
        Name = name;
        Root = new Node<string, SourceFile>
        {
            Key = name,
            Parent = null
        };
    }

    public string Name { get; }
    public Node<string, SourceFile> Root { get; }

    public override string ToString()
    {
        return Name;
    }
}

internal sealed class DisposableRegistry : IDisposable
{
    private readonly Compiler _owner;
    private readonly Registry _previous;
    
    public DisposableRegistry(Compiler owner, Registry registry)
    {
        _owner = owner;
        _previous = owner.Registry;
        owner.Registry = registry;
    }
    
    public void Dispose()
    {
        _owner.Registry = _previous;
    }
}