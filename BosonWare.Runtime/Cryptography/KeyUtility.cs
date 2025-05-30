using System.Security.Cryptography;

namespace BosonWare.Cryptography;

public static class KeyUtility
{
    /// <summary>
    /// Uses PBKDF2 to derive a 256-bit key from the salt and password
    /// </summary>
    /// <param name="salt"></param>
    /// <param name="password"></param>
    /// <param name="iterations"></param>
    /// <param name="derivedKeyLength"></param>
    /// <returns></returns>
    public static byte[] ComputeKey(string salt, string password, int iterations = 10000, int derivedKeyLength = 32)
    {
        var utf8Password = SystemEncoding.UTF8.GetBytes(password);
        var utfSalt = SystemEncoding.UTF8.GetBytes(salt);

        return ComputeKey(utfSalt, utf8Password, iterations, derivedKeyLength);
    }

    /// <summary>
    /// Uses PBKDF2 to derive a 256-bit key from the salt and password
    /// </summary>
    /// <param name="salt"></param>
    /// <param name="password"></param>
    /// <param name="iterations"></param>
    /// <param name="derivedKeyLength"></param>
    /// <returns></returns>
    public static byte[] ComputeKey(ReadOnlySpan<byte> salt, ReadOnlySpan<byte> password, int iterations = 10000, int derivedKeyLength = 32)
    {
        return Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, derivedKeyLength);
    }

    /// <summary>
    /// Uses PBKDF2 to derive a 256-bit key from the salt and password
    /// </summary>
    /// <param name="salt"></param>
    /// <param name="password"></param>
    /// <param name="iterations"></param>
    /// <param name="derivedKeyLength"></param>
    /// <returns></returns>
    public static void ComputeKey(ReadOnlySpan<byte> salt, ReadOnlySpan<byte> password, Span<byte> destination, int iterations = 10000)
    {
        Rfc2898DeriveBytes.Pbkdf2(password, salt, destination, iterations, HashAlgorithmName.SHA256);
    }
}