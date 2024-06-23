namespace Amethyst.Utility;

public static class SelectRecursiveExtension
{
    public static IEnumerable<T> SelectRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
    {
        var stack = new Stack<T>(source);
        while (stack.Count > 0)
        {
            var current = stack.Pop();
            yield return current;
            foreach (var child in selector(current))
            {
                stack.Push(child);
            }
        }
    }
}