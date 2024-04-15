namespace Amethyst;

public static class Utility
{
    public static object ToNbtString(this object obj)
    {
        return obj switch
        {
            string value => $"\"{value}\"",
            bool value => value ? "true" : "false",
            double value => value,
            object[] value => $"[{string.Join(',', value.Select(o => $"{{0:{o.ToNbtString()}}}"))}]",
            _ => obj
        };
    }
}