namespace Amethyst.Model;

/// <summary>Maps a type to the token that is used in Minecraft. This can be virtually anything.</summary>
/// <seealso cref="AmethystOperatorAttribute" />
[AttributeUsage(AttributeTargets.Field)]
public class McfTokenAttribute : Attribute
{
    public string Token { get; }

    public McfTokenAttribute(string token)
    {
        Token = token;
    }
}