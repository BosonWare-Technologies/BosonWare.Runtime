using System.Text;

namespace BosonWare.TUI;

/// <summary>
/// Provides utility methods for writing to and reading from the console with markup processing and input history support.
/// </summary>
[PublicAPI]
public static class TUIConsole
{
    /// <summary>
    /// Writes the specified markup text to the console without a newline, processing any markup codes.
    /// </summary>
    /// <param name="markupText">The text containing markup to write to the console.</param>
    public static void Write(string markupText)
    {
        Console.Write(ProcessMarkup(markupText));
    }
    
    /// <summary>
    /// Writes the specified markup text to the console followed by a newline, processing any markup codes.
    /// </summary>
    /// <param name="markupText">The text containing markup to write to the console.</param>
    public static void WriteLine(string markupText)
    {
        Console.WriteLine(ProcessMarkup(markupText));
    }

    /// <summary>
    /// Processes the given text, converting markup codes to their corresponding ANSI escape sequences.
    /// </summary>
    /// <param name="txt">The text containing markup codes.</param>
    /// <returns>The processed string with ANSI escape sequences.</returns>
    public static string ProcessMarkup(string txt) => AnsiCodes.ProcessMarkup(txt);

    /// <summary>
    /// Reads a line of input from the console with support for input history navigation and markup in the prompt.
    /// </summary>
    /// <param name="prompt">The prompt to display, which may contain markup codes.</param>
    /// <param name="history">A list of previous input strings for history navigation.</param>
    /// <returns>The line of input entered by the user.</returns>
    public static string ReadLineWithHistory(string prompt, IList<string> history)
    {
        prompt = ProcessMarkup(prompt);

        var buffer = new StringBuilder();
        var historyIndex = history.Count;

        Console.Write(prompt);

        while (true) {
            var key = Console.ReadKey(intercept: true);

            if (key.Key == ConsoleKey.Enter) {
                Console.WriteLine();
                var input = buffer.ToString();

                if (!string.IsNullOrWhiteSpace(input)) {
                    history.Add(input);
                }

                return input;
            }
            else if (key.Key == ConsoleKey.Backspace) {
                if (buffer.Length > 0) {
                    buffer.Length--;
                    Console.Write("\b \b");
                }
            }
            else if (key.Key == ConsoleKey.UpArrow) {
                if (history.Count == 0)
                    continue;
                if (historyIndex > 0)
                    historyIndex--;

                buffer.Clear();
                buffer.Append(history[historyIndex]);

                // Clear current line and redraw
                Console.Write($"\r{new string(' ', Console.WindowWidth - 1)}\r{prompt}{buffer}");
            }
            else if (key.Key == ConsoleKey.DownArrow) {
                if (history.Count == 0)
                    continue;
                if (historyIndex < history.Count - 1) {
                    historyIndex++;
                    buffer.Clear();
                    buffer.Append(history[historyIndex]);
                }
                else {
                    historyIndex = history.Count;
                    buffer.Clear();
                }
                Console.Write($"\r{new string(' ', Console.WindowWidth - 1)}\r{prompt}{buffer}");
            }
            else if (!char.IsControl(key.KeyChar)) {
                buffer.Append(key.KeyChar);
                Console.Write(key.KeyChar);
            }
        }
    }
}
