namespace Amethyst;

/// <summary>Provides access to members of a type.</summary>
public interface IMemberAccess
{
    AbstractValue? GetMember(string memberName);
}