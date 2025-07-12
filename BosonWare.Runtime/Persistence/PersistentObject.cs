using System.Text.Json;
using System.Text.Json.Serialization;
using BosonWare.Extensions;

namespace BosonWare.Persistence;

public abstract class PersistentObject<TSelf>(string location) where TSelf : PersistentObject<TSelf>
{
    public delegate TSelf Constructor(string location);

    [JsonIgnore]
    public string Location { get; private set; } = location;

    [JsonConstructor, Obsolete("This constructor is for JsonSerializer only.", true)]
    protected PersistentObject() : this(string.Empty) { }

    public async Task WriteToDiskAsync()
    {
        var json = ((TSelf)this).ToJson(prettyPrint: true);

        await File.WriteAllTextAsync(Location, json);
    }

    public async Task Change(Action<TSelf> action)
    {
        action((TSelf)this);

        await WriteToDiskAsync();
    }
    public static TSelf Create(string location, Constructor constructor)
        => CreateAsync(location, constructor).Result;

    public static async Task<TSelf> CreateAsync(string location, Constructor constructor)
    {
        if (File.Exists(location))
        {
            var json = await File.ReadAllTextAsync(location);

            var obj = JsonSerializer.Deserialize<TSelf>(json) ?? constructor(location);

            obj.Location = location;

            return obj;
        }

        return constructor(location);
    }
}