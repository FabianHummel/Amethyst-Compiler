namespace Amethyst.Utility;

/// <summary>Utility methods for working with the filesystem.</summary>
public static class FilesystemUtility
{
    /// <summary>Copies a directory from source to destination, filtering files based on a predicate.</summary>
    /// <param name="sourceDir">The source directory path.</param>
    /// <param name="destinationDir">The destination directory path.</param>
    /// <param name="predicate">A function that determines whether a file should be copied.</param>
    public static void CopyDirectory(string sourceDir, string destinationDir, Func<string, bool> predicate)
    {
        foreach (var directory in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(directory.Replace(sourceDir, destinationDir));
        }
        
        foreach (var file in Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories))
        {
            if (predicate(file))
            {
                File.Copy(file, file.Replace(sourceDir, destinationDir), true);
            }
        }
    }
}