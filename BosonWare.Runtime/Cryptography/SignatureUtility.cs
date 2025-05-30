using System.Buffers;
using System.Buffers.Text;
using System.Security.Cryptography;

namespace BosonWare.Cryptography;

public static class SignatureUtility
{
    public static string CreateToken(ReadOnlySpan<char> privateKey, string message = "Hello")
    {
        using var rsa = RSA.Create();

        rsa.ImportFromPem(privateKey);

        var data = SystemEncoding.UTF8.GetBytes(message);

        var signedData = rsa.SignData(data, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);

        return Convert.ToBase64String(signedData);
    }

    public static bool CheckSignature(string token, ReadOnlySpan<char> publicKey, string message = "Hello")
    {
        using var rsa = RSA.Create();

        rsa.ImportFromPem(publicKey);

        var signatureBytes = ArrayPool<byte>.Shared.Rent(Base64.GetMaxDecodedFromUtf8Length(token.Length));

        try {
            if (!Convert.TryFromBase64String(token, signatureBytes, out var bytesWritten)) {
                return false;
            }

            var signature = signatureBytes.AsSpan(0, bytesWritten);

            var data = SystemEncoding.UTF8.GetBytes(message);
            
            return rsa.VerifyData(data, signature, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
        }
        finally {
            ArrayPool<byte>.Shared.Return(signatureBytes);
        }
    }
}