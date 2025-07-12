using BosonWare;
using BosonWare.Cryptography;
using BosonWare.Runtime.Lab;
using BosonWare.TUI;

Application.Initialize<IAssemblyMarker>();

Console.WriteLine(Application.PrettyName);

SmartConsole.WriteLine("Hello, World!");

// Adding custom codes.
AnsiCodes.Codes.Add(new AnsiCode("Line", Environment.NewLine));

// Processing markup.
var txt = AnsiCodes.ProcessMarkup("[bright]This is a test of the[/] [red]ANSI[/] codes in [green]BosonWare.Runtime[/].");

Console.WriteLine(txt);

// Cryptography
var key = KeyUtility.ComputeKey("sugar", "passwd");

using IEncryptionService encryptionService = new AesEncryptionService(key);

var cipher = encryptionService.EncryptText("My Secret Api Key");

var deciphered = encryptionService.DecryptText(cipher);

SmartConsole.WriteLine($"[bright]Cipher[/]: [green]{cipher}[/][Line][bright]Deciphered[/]: [magenta]{deciphered}[/]");

Console.ReadKey();
