namespace Amethyst;

public interface ISubstitutable
{
    public sealed void FinalizeCreation(string location)
    {
        FinalizeCreationRecursively($"{location} ");
    }
    
    public void FinalizeCreationRecursively(string substitutionModifierPrefix = "");
}