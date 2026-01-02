using Amethyst.Utility;

namespace Amethyst.Model;

/// <summary>A file extension.</summary>
[AttributeUsage(AttributeTargets.Field)]
public class FileExtensionAttribute : Attribute, IStringAttribute
{
    public string Value { get; }

    public FileExtensionAttribute(string extension)
    {
        Value = extension;
    }
}