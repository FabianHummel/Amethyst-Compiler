namespace Amethyst.Utility;

public static class FilesystemUtility
{
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