using System.Text;

namespace BosonWare.TUI;

public static class TUIConsole
{
    public static string ProcessMarkup(string txt) => AnsiCodes.ProcessMarkup(txt);

    public static string ReadLineWithHistory(string prompt, IList<string> history)
    {
        prompt = ProcessMarkup(prompt);

        var buffer = new StringBuilder();
        int historyIndex = history.Count;

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
