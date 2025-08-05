# BosonWare.Runtime

A lightweight .NET cryptography and encoding utility library providing essential functionality for secure data handling.

## Features

- **Base58 Encoding/Decoding**
    - Standard Base58 implementation
    - Configurable encoding/decoding pipeline

- **Cryptography Services**
    - AES encryption/decryption
    - RSA encryption with PEM support 
    - Ephemeral key generation
    - PBKDF2 key derivation

- **Binary Parsing**
    - Integer parsing (16/32/64 bit)
    - Floating point parsing (32/64 bit)

- **Text User Interface (TUI) Utilities**
    - Rich console output with ANSI color markup (e.g., [GREEN], [RED], [DIM])
    - Thread-safe console writing and reading
    - Animated console output for enhanced user feedback
    - Secure password input with optional confirmation
    - Built-in logging helpers for info, warning, and error messages

## Installation

This package targets .NET 8.0+.
```bash
dotnet add package BosonWare.Runtime
```

## Usage Examples

### Encryption
```csharp
// Base58 encoding
var data = Encoding.UTF8.GetBytes("Hello, World!");
var encoded = Base58.EncodeData(data);

// AES encryption
var key = EphemeralKeys.Get("sessionKey");
using var aes = new AesEncryptionService(key);
var encrypted = aes.Encrypt(data);
```

### TUI
```csharp
// Write a message with ANSI markup.
SmartConsole.WriteLine("[red]This text is red[/]\n[green]This text is green[/]");

// Converting markup to raw ANSI text.
var txt = AnsiCodes.ProcessMarkup("[bright]This is a test of the[/] [red]ANSI[/] codes in [green]BosonWare.Runtime[/].");

Console.WriteLine(txt);

```

## More Documentation
[APPLICATION.README.md](BosonWare.Runtime/APPLICATION.README.md)

[CACHE.README.md](BosonWare.Runtime/CACHE.README.md)

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Author

Developed by CodingBoson at BosonWare, Technologies.

## Release Notes
- Use `DateTime.UtcNow.Ticks` instead of 'DateTime.Now.TimeOfDay'.
