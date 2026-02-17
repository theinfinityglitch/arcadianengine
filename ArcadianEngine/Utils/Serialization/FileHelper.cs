namespace ArcadianEngine.Utils.Serialization;

public static partial class FileHelper
{
    public static string GetExecutablePath()
    {
        return AppContext.BaseDirectory;
    }

    public static string GetPath(params string[] paths)
    {
        var path = Path.Join(paths);

        if (Path.IsPathRooted(path))
        {
            return path;
        }

        return Path.GetFullPath(Path.Join(GetExecutablePath(), path));
    }
}
