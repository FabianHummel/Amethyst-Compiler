namespace Amethyst.Model;

public sealed class Namespace
{
    public required Context Context { get; init; }
    public required string Name { get; init; }
    public Dictionary<string, Registry> Registries { get; } = new();
    
    private readonly TextWriter _writer = new StringWriter();

    public void AddInitCode(string code)
    {
        _writer.WriteLine(code);
    }

    public override string ToString()
    {
        return Name;
    }
}

internal sealed class DisposableNamespace : IDisposable
{
    private readonly Compiler _owner;
    private readonly Namespace _previous;
    
    public DisposableNamespace(Compiler owner, Namespace ns)
    {
        _owner = owner;
        _previous = owner.Namespace;
        owner.Namespace = ns;
    }
    
    public void Dispose()
    {
        _owner.Namespace = _previous;
    }
}