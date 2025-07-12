using System.Text.Json;

namespace BosonWare.Extensions;

public static class JsonExtensions
{
    private static readonly JsonSerializerOptions _prettyOptions = new() {
        WriteIndented = true
    };

    public static string ToJson<T>(this T obj, bool prettyPrint = false)
    {
        if (prettyPrint) {
            return JsonSerializer.Serialize(obj, _prettyOptions);
        }

        return JsonSerializer.Serialize(obj);
    }
}
