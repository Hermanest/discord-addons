using JetBrains.Annotations;

namespace Discord.Addons.Utils;

[PublicAPI]
public static class PlatformTools {
#if LINUX
    public const char PLATFORM_PATH_SEPARATOR = '/';
#else
    public const char PLATFORM_PATH_SEPARATOR = '\\';
#endif
    public const char PROJECT_PATH_SEPARATOR = '\\';

    private static readonly string _projectPathSeparatorStr = PROJECT_PATH_SEPARATOR.ToString();
    private static readonly string _platformPathSeparatorStr = PLATFORM_PATH_SEPARATOR.ToString();

    public static string ConvertPath(string path) {
        return path.Replace(PROJECT_PATH_SEPARATOR, PLATFORM_PATH_SEPARATOR);
    }

    public static string RemovePathSeparators(string path) {
        path = path.Remove(0, path.TakeWhile(Predicate).Count());
        var endCount = path.Reverse().TakeWhile(Predicate).Count();
        return endCount == 0 ? path : path.Remove(path.Length - endCount, endCount);
        
        static bool Predicate(char x) => x is PLATFORM_PATH_SEPARATOR;
    }

    public static string GetDirectoryName(string path) {
        return ConvertPath(Path.GetDirectoryName(path)!);
    }

    public static string CombinePath(params string[] paths) {
        return paths.Aggregate(string.Empty, (current, path) 
            => current + (current != string.Empty ? _platformPathSeparatorStr : string.Empty)
            + RemovePathSeparators(ConvertPath(path)));
    }
}