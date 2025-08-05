# BosonWare Application Class Documentation

## Overview

The `BosonWare.Application` class provides a centralized application configuration and path management system for .NET applications. It works in conjunction with the `ApplicationAttribute` to define application metadata and automatically manage application data directories.

## Classes

### `ApplicationAttribute`

A sealed attribute class that defines application metadata at the assembly level.

#### Constructor
```csharp
public ApplicationAttribute(string name)
```

**Parameters:**
- `name` (string): The human-readable name of the application

#### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Name` | `string` | Constructor parameter | The display name of the application |
| `Folder` | `SpecialFolder` | `SpecialFolder.UserProfile` | The base folder where application data will be stored |
| `Version` | `string` | `"1.0.0"` | The version string of the application |

#### Usage Example
```csharp
[assembly: Application("MyApp", Folder = SpecialFolder.ApplicationData, Version = "2.1.0")]
```

### `Application` Static Class

Provides static methods and properties for application configuration and path management.

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Version` | `Version` | The parsed version of the application (default: 1.0.0) |
| `PrettyName` | `string` | The human-readable application name (lazy-loaded from assembly or AppDomain) |
| `UnixFolderName` | `string` | The Unix-style folder name (prefixed with dot) |
| `DataPath` | `string` | The full path to the application's data directory |

#### Methods

##### `Initialize<AssemblyMarker>()`
```csharp
public static void Initialize<AssemblyMarker>()
```

Initializes the application configuration by reading the `ApplicationAttribute` from the specified assembly.

**Type Parameters:**
- `AssemblyMarker`: A type from the assembly containing the `ApplicationAttribute`

**Example:**
```csharp
// In your main assembly with [assembly: Application("MyApp")]
Application.Initialize<Program>();
```

##### `GetPath(params string[] relativePaths)`
```csharp
public static string GetPath(params string[] relativePaths)
```

Returns the full path to a file within the application data directory, creating intermediate directories if they don't exist.

**Parameters:**
- `relativePaths`: Variable number of path segments relative to the application data directory

**Returns:** Full file path as string

**Example:**
```csharp
string configPath = Application.GetPath("config", "settings.json");
// Returns: /home/user/.MyApp/config/settings.json (on Unix)
```

##### `GetDirectory(params string[] relativePaths)`
```csharp
public static string GetDirectory(params string[] relativePaths)
```

Returns the full path to a directory within the application data directory, creating it if it doesn't exist.

**Parameters:**
- `relativePaths`: Variable number of path segments relative to the application data directory

**Returns:** Full directory path as string

**Example:**
```csharp
string logsDir = Application.GetDirectory("logs", "2024");
// Returns: /home/user/.MyApp/logs/2024/ (and creates the directory)
```

## Configuration

### Environment Variable Override

The application data path can be overridden using the `APPLICATION_PATH` environment variable:

```bash
export APPLICATION_PATH="/custom/app/path"
```

When set, this path will be used instead of the default location based on the `SpecialFolder`.

### Default Behavior

- **PrettyName**: Uses `AppDomain.CurrentDomain.FriendlyName` if no `ApplicationAttribute` is found
- **UnixFolderName**: Automatically prefixes the pretty name with a dot (e.g., ".MyApp")
- **DataPath**: Combines the special folder path with the Unix folder name
- **Directory Creation**: Automatically creates directories when accessing paths

## Usage Patterns

### Basic Setup
```csharp
// 1. Define the attribute at assembly level
[assembly: Application("MyApplication", Version = "1.2.3")]

// 2. Initialize in your main method
static void Main()
{
    Application.Initialize<Program>();
    
    // Now you can use the application paths
    string configFile = Application.GetPath("config.json");
    string dataDir = Application.GetDirectory("data");
}
```

### Advanced Configuration
```csharp
[assembly: Application("MyApp", 
    Folder = SpecialFolder.LocalApplicationData, 
    Version = "2.0.0-beta")]

static void Main()
{
    Application.Initialize<Program>();
    
    Console.WriteLine($"App: {Application.PrettyName} v{Application.Version}");
    Console.WriteLine($"Data Path: {Application.DataPath}");
    
    // Create nested directory structure
    string cacheDir = Application.GetDirectory("cache", "images");
    string logFile = Application.GetPath("logs", $"{DateTime.Now:yyyy-MM-dd}.log");
}
```

## Thread Safety

The `Application` class uses lazy initialization with backing fields. The `Initialize<T>()` method should be called once during application startup before accessing other members from multiple threads.

## Platform Considerations

- **Unix/Linux**: Creates hidden directories with dot prefix (e.g., `.myapp`)
- **Windows**: Uses standard application data folders
- **Cross-platform**: Leverages `System.Environment.SpecialFolder` for platform-appropriate paths