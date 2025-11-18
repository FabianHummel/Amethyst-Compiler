using Amethyst.Model;

namespace Amethyst.Utility;

/// <summary>Represents a node in a tree structure. Allows traversal from any node to the root by
/// following parent references.</summary>
/// <typeparam name="TKey">The type of the key used to identify each node. Must be non-nullable.</typeparam>
/// <typeparam name="TValue">The type of the value associated with each node.</typeparam>
public class Node<TKey, TValue> where TKey : notnull
{
    /// <summary>The parent node of the current node. Null if this node is the root.</summary>
    public required Node<TKey, TValue>? Parent { get; init; }

    /// <summary>The key identifying this node.</summary>
    public required TKey Key { get; init; }

    /// <summary>A list of all segments (or keys) from the root node to this node.</summary>
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

    /// <summary>Creates a tree of nodes from an array of path segments.</summary>
    /// <param name="pathSegments">An array of path segments representing the keys of the nodes.</param>
    /// <returns>The root node of the created tree, or null if the input array is empty.</returns>
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

    /// <summary>Returns a string representation of the node's path by joining its segments with " > ".</summary>
    /// <returns>A string representing the path from the root to this node.</returns>
    public override string ToString()
    {
        return string.Join(" > ", Segments);
    }
}