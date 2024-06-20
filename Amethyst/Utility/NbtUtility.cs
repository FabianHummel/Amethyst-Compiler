namespace Amethyst.Utility;

public static class NbtUtility
{
    public static object ToNbtString(this object obj)
    {
        return obj switch
        {
            string value => $"\"{value}\"",
            bool value => value ? "true" : "false",
            double value => value,
            object[] value => $"[{string.Join(',', value.Select(o => $"{{_:{o.ToNbtString()}}}"))}]",
            _ => obj
        };
    }
}