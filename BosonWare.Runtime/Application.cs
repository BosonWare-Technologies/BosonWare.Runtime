using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using static System.Environment;

namespace BosonWare;

/// <summary>
/// Provides centralized application configuration and path management functionality.
/// This static class automatically manages application metadata, versioning, and file system paths
/// based on assembly-level <see cref="ApplicationAttribute"/> configuration.
/// </summary>
/// <remarks>
/// The Application class uses lazy initialization for optimal performance and supports
/// environment variable overrides for deployment flexibility. All path operations
/// automatically create necessary directories.
/// </remarks>
public static class Application
{
    /// <summary>
    /// Gets the parsed version of the application.
    /// Initialized from the <see cref="ApplicationAttribute.Version"/> property during <see cref="Initialize{AssemblyMarker}"/>.
    /// </summary>
    /// <value>A <see cref="Version"/> object representing the application version</value>
    public static Version Version { get; private set; } = Version.Parse("1.0.0");

    /// <summary>
    /// Gets the human-readable name of the application.
    /// Lazily loaded from <see cref="ApplicationAttribute.Name"/> or falls back to <see cref="AppDomain.FriendlyName"/>.
    /// </summary>
    /// <value>The application's display name</value>
    [field: MaybeNull] [field: AllowNull]
    public static string PrettyName {
        get => field ??= AppDomain.CurrentDomain.FriendlyName;
        private set;
    }

    /// <summary>
    /// Gets the Unix-style folder name for the application data directory.
    /// Automatically prefixes the <see cref="PrettyName"/> with a dot for hidden directory convention.
    /// </summary>
    /// <value>A string in the format ".ApplicationName"</value>
    [field: MaybeNull] [field: AllowNull]
    public static string UnixFolderName {
        get => field ??= $".{PrettyName}";
        internal set;
    }

    /// <summary>
    /// Gets the full path to the application's data directory.
    /// Combines the base folder from <see cref="ApplicationAttribute.Folder"/> with <see cref="UnixFolderName"/>.
    /// Can be overridden using the APPLICATION_PATH environment variable.
    /// </summary>
    /// <value>The absolute path to the application's data directory</value>
    [field: MaybeNull] [field: AllowNull]
    public static string DataPath {
        get => field ??= GetAppFolder(SpecialFolder.UserProfile);
        private set;
    }

    /// <summary>
    /// Initializes the application configuration by reading the <see cref="ApplicationAttribute"/> 
    /// from the specified assembly. This method should be called once during application startup.
    /// </summary>
    /// <typeparam name="TAssemblyMarker">A type from the assembly containing the ApplicationAttribute</typeparam>
    /// <remarks>
    /// If no <see cref="ApplicationAttribute"/> is found, the application will use default values.
    /// This method is safe to call multiple times but should typically be called once in Main().
    /// </remarks>
    /// <example>
    /// <code>
    /// // In your main method
    /// Application.Initialize&lt;Program&gt;();
    /// </code>
    /// </example>
    public static void Initialize<TAssemblyMarker>()
    {
        var attribute = typeof(TAssemblyMarker).Assembly
            .GetCustomAttribute<ApplicationAttribute>();

        if (attribute is null)
            return;

        PrettyName = attribute.Name;
        Version = Version.Parse(attribute.Version);
        DataPath = GetAppFolder(attribute.Folder);
    }

    /// <summary>
    /// Gets the full path to a file within the application data directory.
    /// Automatically creates any intermediate directories that don't exist.
    /// </summary>
    /// <param name="relativePaths">Variable number of path segments relative to the application data directory</param>
    /// <returns>The complete file path as a string</returns>
    /// <exception cref="ArgumentException">Thrown when path segments contain invalid characters</exception>
    /// <exception cref="DirectoryNotFoundException">Thrown when unable to create intermediate directories</exception>
    /// <example>
    /// <code>
    /// string configPath = Application.GetPath("config", "settings.json");
    /// // Returns: /home/user/.MyApp/config/settings.json
    /// </code>
    /// </example>
    public static string GetPath(params string[] relativePaths)
    {
        var fullPath = Path.Combine(DataPath, Path.Combine(relativePaths));

        var directory = Path.GetDirectoryName(fullPath)!;

        if (!Directory.Exists(directory)) {
            Directory.CreateDirectory(directory);
        }

        return fullPath;
    }

    /// <summary>
    /// Gets the full path to a directory within the application data directory.
    /// Automatically creates the directory and any intermediate directories if they don't exist.
    /// </summary>
    /// <param name="relativePaths">Variable number of path segments relative to the application data directory</param>
    /// <returns>The complete directory path as a string</returns>
    /// <exception cref="ArgumentException">Thrown when path segments contain invalid characters</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when lacking permissions to create directories</exception>
    /// <example>
    /// <code>
    /// string logsDir = Application.GetDirectory("logs", "2024");
    /// // Returns: /home/user/.MyApp/logs/2024/ (and creates the directory)
    /// </code>
    /// </example>
    public static string GetDirectory(params string[] relativePaths)
    {
        var fullPath = Path.Combine(DataPath, Path.Combine(relativePaths));

        if (!Directory.Exists(fullPath)) {
            Directory.CreateDirectory(fullPath);
        }

        return fullPath;
    }

    /// <summary>
    /// Determines the application's data folder path based on the specified special folder.
    /// Supports environment variable override via APPLICATION_PATH.
    /// </summary>
    /// <param name="folder">The base special folder to use for the application data</param>
    /// <returns>The full path to the application's data folder</returns>
    /// <remarks>
    /// If the APPLICATION_PATH environment variable is set and not empty, it will be used
    /// instead of the computed path based on the special folder.
    /// </remarks>
    private static string GetAppFolder(SpecialFolder folder)
    {
        var path = GetEnvironmentVariable("APPLICATION_PATH");

        if (path is null or "") {
            path = Path.Combine(GetFolderPath(folder), UnixFolderName);
        }

        return path;
    }
}
