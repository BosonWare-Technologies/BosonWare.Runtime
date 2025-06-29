using System.Buffers;
using System.Buffers.Text;
using System.Security.Cryptography;

namespace BosonWare.Cryptography;

/// <summary>
/// Provides utility methods for creating and verifying digital signatures using RSA cryptography.
/// </summary>
public static class SignatureUtility
{
    /// <summary>
    /// Creates a digital signature (token) for the specified message using the provided RSA private key in PEM format.
    /// </summary>
    /// <param name="privateKey">The RSA private key in PEM format as a read-only character span.</param>
    /// <param name="message">The message to sign. Defaults to "Hello" if not specified.</param>
    /// <returns>A Base64-encoded string representing the digital signature of the message.</returns>
    public static string CreateToken(ReadOnlySpan<char> privateKey, string message = "Hello")
    {
        using var rsa = RSA.Create();

        rsa.ImportFromPem(privateKey);

        var data = SystemEncoding.UTF8.GetBytes(message);

        var signedData = rsa.SignData(data, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);

        return Convert.ToBase64String(signedData);
    }

    /// <summary>
    /// Verifies the digital signature (token) of a message using the provided RSA public key in PEM format.
    /// </summary>
    /// <param name="token">The Base64-encoded digital signature to verify.</param>
    /// <param name="publicKey">The RSA public key in PEM format as a read-only character span.</param>
    /// <param name="message">The original message that was signed. Defaults to "Hello" if not specified.</param>
    /// <returns><c>true</c> if the signature is valid for the given message and public key; otherwise, <c>false</c>.</returns>
    public static bool CheckSignature(string token, ReadOnlySpan<char> publicKey, string message = "Hello")
    {
        using var rsa = RSA.Create();

        rsa.ImportFromPem(publicKey);

        var signatureBytes = ArrayPool<byte>.Shared.Rent(Base64.GetMaxDecodedFromUtf8Length(token.Length));

        try
        {
            if (!Convert.TryFromBase64String(token, signatureBytes, out var bytesWritten))
            {
                return false;
            }

            var signature = signatureBytes.AsSpan(0, bytesWritten);

            var data = SystemEncoding.UTF8.GetBytes(message);

            return rsa.VerifyData(
                data,
                signature,
                HashAlgorithmName.SHA512,
                RSASignaturePadding.Pkcs1);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(signatureBytes);
        }
    }
}