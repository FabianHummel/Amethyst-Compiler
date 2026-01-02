namespace Amethyst;

public interface ISubstitutable
{
    public void FinalizeCreation(string location)
    {
        FinalizeCreationRecursively($"{location} ");
    }
    
    public void FinalizeCreationRecursively(string substitutionModifierPrefix = "");
}