namespace Amethyst.Utility;

public static class StringGenerator
{
    private static readonly Random _random = new();
    
    public static string GenerateRandomString(int length)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyz";
        return new string(Enumerable.Repeat(chars, length).Select(s => s[_random.Next(s.Length)]).ToArray());
    }
}