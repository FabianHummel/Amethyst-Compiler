namespace Amethyst.Model;

public enum ScopeType
{
    /// <summary>
    /// A datapack function, ending in .mcfunction
    /// </summary>
    [FileExtension(".mcfunction")]
    Function,
    
    /// <summary>
    /// A resourcepack resource, ending in .json
    /// </summary>
    [FileExtension(".json")]
    Resource,
    
    /// <summary>
    /// A resourcepack image, ending in .png
    /// </summary>
    [FileExtension(".png")]
    Image,
    
    /// <summary>
    /// A resourcepack sound, ending in .ogg
    /// </summary>
    [FileExtension(".ogg")]
    Sound,
}