namespace Amethyst;

/// <summary>Provides access to members of a type.</summary>
public interface IMemberAccess
{
    /// <summary>Gets the member with the specified constant name <paramref name="memberName" />. The name
    /// is kept a constant, because member evaluation during runtime is too complex.</summary>
    /// <param name="memberName">The name of the member to access on the object.</param>
    /// <returns>The member on the object with the specified <paramref name="memberName" />.</returns>
    AbstractValue? GetMember(string memberName);
}