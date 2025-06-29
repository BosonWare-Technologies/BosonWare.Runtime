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
