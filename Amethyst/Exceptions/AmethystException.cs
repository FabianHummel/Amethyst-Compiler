namespace Amethyst;

/// <summary>Base exception for all compilation-related errors.</summary>
public class AmethystException : Exception
{
    protected AmethystException(string message) : base(message)
    {
    }
}