using System.Diagnostics.CodeAnalysis;

namespace Amethyst.Utility;

public class Node<TKey, TValue> where TKey : notnull
{
    private readonly Dictionary<TKey, Leaf<TKey, TValue>> _leaves = new();
    private readonly Dictionary<TKey, Node<TKey, TValue>> _nodes = new();

    public required Node<TKey, TValue>? Parent { get; init; }
    public required TKey Key { get; init; }
    
    public IReadOnlyDictionary<TKey, Leaf<TKey, TValue>> Leaves => _leaves;
    public IReadOnlyDictionary<TKey, Node<TKey, TValue>> Nodes => _nodes;

    public TKey[] Segments
    {
        get
        {
            var segments = new List<TKey>();
            var current = this;
            while (current != null)
            {
                segments.Add(current.Key);
                current = current.Parent;
            }
            segments.Reverse();
            return segments.ToArray();
        }
    }

    public Node<TKey, TValue> CreateOrGetNode(TKey[] segments)
    {
        if (segments.Length == 1) return this;

        var current = this;
        for (int i = 0; i < segments.Length - 1; i++)
        {
            if (!Nodes.TryGetValue(segments[i], out var node))
            {
                node = new Node<TKey, TValue> { Key = segments[i], Parent = current };
                current.AddNode(segments[i], node);
            }
            current = node;
        }
        return current;
    }
    
    public IEnumerable<Leaf<TKey, TValue>> GetAllLeaves()
    {
        return _leaves.Values.Concat(_nodes.Values.SelectMany(n => n.GetAllLeaves()));
    }

    public bool TryAddLeaf(TKey key, TValue value)
    {
        var leaf = new Leaf<TKey, TValue> { Key = key, Value = value, Parent = this };
        return _leaves.TryAdd(key, leaf);
    }
    
    public void AddLeaf(TKey key, TValue value)
    {
        var leaf = new Leaf<TKey, TValue> { Key = key, Value = value, Parent = this };
        _leaves.Add(key, leaf);
    }
    
    public bool TryAddNode(TKey key, Node<TKey, TValue> node)
    {
        return _nodes.TryAdd(key, node);
    }
    
    public void AddNode(TKey key, Node<TKey, TValue> node)
    {
        _nodes.Add(key, node);
    }

    public bool TryTraverse(TKey[] segments, [NotNullWhen(true)] out Node<TKey, TValue>? node)
    {
        node = this;
        foreach (var segment in segments)
        {
            if (!Nodes.TryGetValue(segment, out node))
            {
                return false;
            }
        }
        return true;
    }

    public override string ToString()
    {
        return string.Join('/', Segments);
    }
}

public class Leaf<TKey, TValue> where TKey : notnull
{
    public required TKey Key { get; init; }
    public required TValue Value { get; init; }
    public required Node<TKey, TValue>? Parent { get; init; }

    public override string ToString()
    {
        return $"{Key}: {Value} ({Parent}/{Key})";
    }
}