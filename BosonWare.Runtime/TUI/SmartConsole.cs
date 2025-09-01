using System.Runtime.CompilerServices;
using System.Text;
// ReSharper disable MemberHidesStaticFromOuterClass

namespace BosonWare.TUI;

/// <summary>
/// Provides thread-safe console operations.
/// </summary>
[PublicAPI]
public static class SmartConsole
{
    private sealed class ThreadedConsole
    {
        private volatile bool _isLocked;

        /// <summary>
        /// Locks the console to prevent writing.
        /// </summary>
        public void Lock() => _isLocked = true;

        /// <summary>
        /// Unlocks the console to allow writing.
        /// </summary>
        public void Unlock() => _isLocked = false;

        /// <summary>
        /// Writes a line to the console with the specified color.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="color">The color of the message.</param>
        public void WriteLine(string message, ConsoleColor? color = null)
        {
            if (_isLocked)
                return;

            Write(message, color);
            Console.WriteLine();
        }

        /// <summary>
        /// Writes a message to the console with the specified color.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="color">The color of the message.</param>
        public void Write(string message, ConsoleColor? color = null)
        {
            if (_isLocked)
                return;

            if (color is not null) {
                var previousColor = Console.ForegroundColor;
                Console.ForegroundColor = color.Value;
                Console.Write(AnsiCodes.ProcessMarkup(message));
                Console.ForegroundColor = previousColor;

                return;
            }

            Console.Write(AnsiCodes.ProcessMarkup(message));
        }

        /// <summary>
        /// Reads a line from the console with a prompt and the specified color.
        /// </summary>
        /// <param name="prompt">The prompt to display.</param>
        /// <param name="color">The color of the prompt.</param>
        /// <returns>The input from the console.</returns>
        public string ReadLine(string prompt, ConsoleColor? color = null)
        {
            if (_isLocked)
                return string.Empty;

            Write(prompt, color);
            return Console.ReadLine() ?? string.Empty;
        }
    }

    private static readonly ThreadedConsole ConsoleInstance = new();

    /// <summary>
    /// Locks the console to prevent writing.
    /// </summary>
    public static void Lock()
    {
        lock (ConsoleInstance) {
            ConsoleInstance.Lock();
        }
    }

    /// <summary>
    /// Unlocks the console to allow writing.
    /// </summary>
    public static void Unlock()
    {
        lock (ConsoleInstance) {
            ConsoleInstance.Unlock();
        }
    }

    public static void WriteLineAnimated(object? message, int delay = 40)
        => WriteLineAnimatedAsync(message, delay).Wait();

    public static async Task WriteLineAnimatedAsync(object? message, int delay = 40)
    {
        var msg = AnsiCodes.ProcessMarkup(FormatObjectAsString(message));

        foreach (var c in msg.Split(' ',
            StringSplitOptions.RemoveEmptyEntries
            | StringSplitOptions.TrimEntries)) {
            await Task.Delay(delay);

            Console.Write(c);
            Console.Write(' ');
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Writes a line to the console with the specified color.
    /// </summary>
    /// <param name="message">The message to write.</param>
    /// <param name="color">The color of the message.</param>
    public static void WriteLine(object? message, ConsoleColor? color = null)
    {
        lock (ConsoleInstance) {
            ConsoleInstance.WriteLine(FormatObjectAsString(message), color);
        }
    }

    /// <summary>
    /// Writes a message to the console with the specified color.
    /// </summary>
    /// <param name="message">The message to write.</param>
    /// <param name="color">The color of the message.</param>
    public static void Write(object? message, ConsoleColor? color = null)
    {
        lock (ConsoleInstance) {
            ConsoleInstance.Write(FormatObjectAsString(message), color);
        }
    }

    /// <summary>
    /// Reads a line from the console with a prompt and the specified color.
    /// </summary>
    /// <param name="prompt">The prompt to display.</param>
    /// <param name="color">The color of the prompt.</param>
    /// <returns>The input from the console.</returns>
    public static string ReadLine(string prompt, ConsoleColor? color = null)
    {
        lock (ConsoleInstance) {
            return ConsoleInstance.ReadLine(prompt, color);
        }
    }

    public static string ReadPassword(
        string prompt,
        bool confirm = false,
        string confirmPrompt = "Confirm password: ")
    {
        Console.Write(prompt);

        var builder = new StringBuilder();

        while (true) {
            var key = Console.ReadKey(intercept: true);

            if (key.Key == ConsoleKey.Enter) {
                Console.WriteLine();

                break;
            }

            if (key.Key == ConsoleKey.Backspace && builder.Length > 0) {
                builder.Remove(builder.Length - 1, 1);

                continue;
            }

            if (char.IsAscii(key.KeyChar)) {
                builder.Append(key.KeyChar);
            }
        }

        if (!confirm)
            return builder.ToString();

        var password = builder.ToString();

        while (true) {
            var confirmationPassword = ReadPassword(confirmPrompt, confirm: false);

            if (confirmationPassword == password)
                return password;

            Console.WriteLine("The confirmation password does not match the original one. Try again.");

        }
    }

    /// <summary>
    /// Logs an info message to the console.
    /// </summary>
    /// <param name="message">The message to log.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void LogInfo(object? message) => Log("INFO", message, ConsoleColor.Green);

    /// <summary>
    /// Logs a warning message to the console.
    /// </summary>
    /// <param name="message">The message to log.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void LogWarning(object? message) => Log("WARNING", message, ConsoleColor.Yellow);

    /// <summary>
    /// Logs an error message to the console.
    /// </summary>
    /// <param name="message">The message to log.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void LogError(object? message) => Log("ERROR", message, ConsoleColor.Red);

    /// <summary>
    /// Logs a message to the console with the specified type and color.
    /// </summary>
    /// <param name="type">The type of the message.</param>
    /// <param name="message">The message to log.</param>
    /// <param name="color">The color of the message.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Log(string type, object? message, ConsoleColor color)
    {
        var formatted = (message?.ToString() ?? "NULL")
            .Replace(Environment.NewLine, Environment.NewLine + new string(' ', type.Length + 3));

        Write($"[{type}] ", color);
        WriteLine(formatted);
    }

    internal static string FormatObjectAsString(object? message)
        => message?.ToString() ?? "[Violet]NULL[/]";
}
