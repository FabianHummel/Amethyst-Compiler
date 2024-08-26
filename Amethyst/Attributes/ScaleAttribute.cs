namespace Amethyst.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class ScaleAttribute : Attribute
{
    public int Scale { get; }
    
    public ScaleAttribute(int scale)
    {
        Scale = scale;
    }
}
