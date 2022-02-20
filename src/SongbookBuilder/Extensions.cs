using System.IO;

namespace SongbookBuilder;

public static class Extensions
{
    public static string IfEndsWithReplace(this string current, string toReplace, string replacement)
    {
        if (current.EndsWith(toReplace))
            return current.Replace(toReplace, replacement);

        return current;
    }
    
    public static void CopyDirectory(string sourcePath, string targetPath)
    {
        //Now Create all of the directories
        foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
        }

        if (Directory.Exists(targetPath) == false)
            Directory.CreateDirectory(targetPath);

        //Copy all the files & Replaces any files with the same name
        foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
        {
            File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
        }
    }
}