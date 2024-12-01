namespace Amethyst.Utility;

public static class NbtUtility
{
    public static object ToNbtString(this object obj)
    {
        return obj switch
        {
            string value => $"\"{value}\"",
            bool value => value ? "true" : "false",
            object[] value => $"[{string.Join(',', value.Select(o => $"{{_:{o.ToNbtString()}}}"))}]",
            _ => obj
        };
    }
    
    public static int ToNbtNumber(this object obj)
    {
        return obj switch
        {
            string value => value.Length,
            bool value => value ? 1 : 0,
            double value => (int) (value * 100),
            int value => value,
            object[] value => value.Length,
            _ => throw new ArgumentException("Invalid type for NBT number conversion.")
        };
    }
    
    public static object RandomNbtValue(Random random)
    {
        return random.Next(4) switch
        {
            0 => random.NextDouble() * 10 - 5,
            1 => random.Next(2) == 0,
            2 => random.Next(10) - 5,
            3 => string.Join("", Enumerable.Range(0, new Random().Next(10)).Select(_ => "A")),
            _ => throw new InvalidOperationException()
        };
    }
}