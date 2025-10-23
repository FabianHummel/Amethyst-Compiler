using Amethyst.Model;

namespace Amethyst.Utility;

public class Node<TKey, TValue> where TKey : notnull
{
    public required Node<TKey, TValue>? Parent { get; init; }
    public required TKey Key { get; init; }
    
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

    public static Node<string, SourceFile>? CreateTree(string[] pathSegments)
    {
        if (pathSegments.Length == 0)
        {
            return null;
        }

        return new Node<string, SourceFile>
        {
            Key = pathSegments[^1],
            Parent = CreateTree(pathSegments[..^1])
        };
    }

    public override string ToString()
    {
        return string.Join(" > ", Segments);
    }
}