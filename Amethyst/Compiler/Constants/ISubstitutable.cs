namespace Amethyst;

public interface ISubstitutable
{
    public void SubstituteRecursively(Compiler compiler, string substitutionModifierPrefix = "");
}