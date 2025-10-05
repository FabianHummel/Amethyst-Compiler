namespace Tests;

[AttributeUsage(AttributeTargets.Method)]
public class LinkAttribute : Attribute
{
    public string Path { get; }
    
    public LinkAttribute(string path)
    {
        if (!path.Contains(':'))
        {
            path = $"{Constants.DefaultTestNamespace}:{path}";
        }
        
        Path = path;
    }
}