namespace AudioSubMerger.Helpers;

public static class GeneralHelper
{
    public static IEnumerable<FileInfo> GetFilesFromDirectoryWithExtensions(string directoryPath, IEnumerable<string> extensions)
    {
        var extensionsList = extensions.ToList();
        return new DirectoryInfo(directoryPath).GetFiles().Where(x => extensionsList.Contains(x.Extension));
    }
    
    // Check if path is absolute
    public static bool IsAbsolutePath(string path)
    {
        return Path.IsPathRooted(path) && !Path.GetPathRoot(path).Equals(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal);
    }
    
    // Get directories from path
    public static IEnumerable<DirectoryInfo> GetDirectoriesFromPath(string path)
    {
        var directoryInfo = new DirectoryInfo(path);
        if (!directoryInfo.Exists)
        {
            return Enumerable.Empty<DirectoryInfo>();
        }
        
        return new DirectoryInfo(path).GetDirectories();
    }
}