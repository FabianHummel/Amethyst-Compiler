namespace Tests;

[AttributeUsage(AttributeTargets.Method)]
public class LinkAttribute : Attribute
{
    private string _path { get; }
    
    public LinkAttribute(string path)
    {
        _path = path;
    }

    public string Path
    {
        get
        {
            string path = _path;
            if (!path.Contains(':'))
            {
                path = $"{Constants.DefaultTestNamespace}:{path}";
            }
            return path;
        }
    }

    public IEnumerable<string> Segments
    {
        get
        {
            var segments = Path.Split('/');
            return [..segments[0].Split(':'), ..segments[1..]];
        }
    }
}