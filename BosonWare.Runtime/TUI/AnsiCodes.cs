using System.Text;

namespace BosonWare.TUI;

[PublicAPI]
public readonly struct AnsiCode(string human, string code)
{
    public readonly string Human = human;
    public readonly string Code = code;
}

[PublicAPI]
public static class AnsiCodes
{
    public static List<AnsiCode> Codes { get; } = [
        new AnsiCode("BRIGHT", "\x1b[1m"),
        new AnsiCode("DIM", "\x1b[2m"),
        new AnsiCode("GREEN", "\x1b[32m"),
        new AnsiCode("YELLOW", "\x1b[33m"),
        new AnsiCode("RED", "\x1b[31m"),
        new AnsiCode("DARKRED", "\x1b[38;2;139;0;0m"), // Changed to a darker red
        new AnsiCode("CRIMSON", "\x1b[38;2;220;20;60m"), // Changed to crimson
        new AnsiCode("CYAN", "\x1b[36m"),
        new AnsiCode("PURPLE", "\x1b[38;2;135;2;250m"),
        new AnsiCode("MAGENTA", "\x1b[38;2;238;130;238m"),
        new AnsiCode("VIOLET", "\x1b[38;2;123;104;238m"),
        new AnsiCode("/", "\x1b[0m"),
    ];

    public static string ProcessMarkup(string message)
    {
        if (string.IsNullOrEmpty(message))
            return message;

        var builder = new StringBuilder();
        var i = 0;
        while (i < message.Length) {
            switch (message[i]) {
                case '\\' when i + 1 < message.Length && (message[i + 1] == '[' || message[i + 1] == ']'):
                    // Escaped bracket
                    builder.Append(message[i + 1]);
                    i += 2;
                    break;
                case '[': {
                    var end = message.IndexOf(']', i + 1);
                    if (end > i + 1) {
                        var codeName = message.AsSpan(i + 1, end - i - 1);
                        if (TryGetAnsiCode(codeName, out var ansiCode)) {
                            builder.Append(ansiCode.Code);
                            i = end + 1;
                            continue;
                        }
                    }
                    // Not a valid code, treat as normal char
                    builder.Append('[');
                    i++;
                    break;
                }
                default:
                    builder.Append(message[i]);
                    i++;
                    break;
            }
        }
        return builder.ToString();
    }

    public static bool TryGetAnsiCode(ReadOnlySpan<char> name, out AnsiCode code)
    {
        foreach (var ansiCode in Codes) {
            if (!ansiCode.Human.AsSpan().Equals(name, StringComparison.OrdinalIgnoreCase))
                continue;

            code = ansiCode;
            return true;
        }
        code = default;
        return false;
    }
}
