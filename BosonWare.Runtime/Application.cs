using System.Reflection;
using static System.Environment;

namespace BosonWare;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
public sealed class ApplicationAttribute(string name) : Attribute
{
    public string Name { get; set; } = name;

    public SpecialFolder Folder { get; set; } = SpecialFolder.UserProfile;

    public string? Version { get; set; }
}

public static class Application
{
    public static Version Version { get; private set; } = Version.Parse("1.0.0");

    public static string PrettyName { get; private set; } = "MyApp";

    public static string UnixFolderName { get; private set; } = "MyApp";

    public static string DataPath
        => field ??= Path.Combine(GetFolderPath(SpecialFolder.UserProfile), UnixFolderName);

    public static void Initialize<AssemblyMarker>()
    {
        var attribute = typeof(AssemblyMarker).Assembly.GetCustomAttribute<ApplicationAttribute>();

        string? version;

        if (attribute is not null) {
            PrettyName = attribute.Name;
            UnixFolderName = "." + attribute.Name;

            version = attribute.Version;
        }
        else {
            // Fallback to the AppDomain's FriendlyName
            var appName = AppDomain.CurrentDomain.FriendlyName;

            PrettyName = appName;
            UnixFolderName = "." + appName;

            version = typeof(AssemblyMarker).Assembly.GetName().Version?.ToString();
        }

        Version = Version.Parse(version is not null or "" ? version : "1.0.0");
    }

    public static string GetPath(params string[] relativePaths)
    {
        var fullPath = Path.Combine(DataPath, Path.Combine(relativePaths));

        var directory = Path.GetDirectoryName(fullPath)!;

        if (!Directory.Exists(directory)) {
            Directory.CreateDirectory(directory);
        }

        return fullPath;
    }

    public static string GetDirectory(params string[] relativePaths)
    {
        var fullPath = Path.Combine(DataPath, Path.Combine(relativePaths));

        if (!Directory.Exists(fullPath)) {
            Directory.CreateDirectory(fullPath);
        }

        return fullPath;
    }
}
