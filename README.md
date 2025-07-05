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

This project targets .NET 8.0.

## Usage Example

```csharp
// Base58 encoding
var data = Encoding.UTF8.GetBytes("Hello, World!");
var encoded = Base58.EncodeData(data);

// AES encryption
var key = EphemeralKeys.Get("sessionKey");
using var aes = new AesEncryptionService(key);
var encrypted = aes.Encrypt(data);
```

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Author

Developed by CodingBoson at BosonWare, Technologies.
