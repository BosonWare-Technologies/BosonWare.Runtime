namespace BosonWare;

/// <summary>
/// Defines application metadata at the assembly level for automatic configuration of the Application class.
/// Use this attribute to specify the application name, version, and data folder location.
/// </summary>
/// <param name="name">The human-readable name of the application</param>
/// <example>
/// <code>
/// [assembly: Application("MyApp", Folder = SpecialFolder.ApplicationData, Version = "2.1.0")]
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Assembly)]
public sealed class ApplicationAttribute(string name) : Attribute
{
    /// <summary>
    /// Gets or sets the human-readable display name of the application.
    /// This name is used for directory creation and display purposes.
    /// </summary>
    /// <value>The application's display name</value>
    public string Name { get; set; } = name;

    /// <summary>
    /// Gets or sets the base system folder where application data will be stored.
    /// Defaults to the user's profile directory if not specified.
    /// </summary>
    /// <value>A <see cref="Environment.SpecialFolder"/> enumeration value specifying the base location</value>
    public Environment.SpecialFolder Folder { get; set; } = Environment.SpecialFolder.UserProfile;

    /// <summary>
    /// Gets or sets the version string of the application.
    /// Must be a valid version format parseable by <see cref="Version.Parse(string)"/>.
    /// </summary>
    /// <value>A version string in the format "major.minor.build" or similar</value>
    public string Version { get; set; } = "1.0.0";
}
