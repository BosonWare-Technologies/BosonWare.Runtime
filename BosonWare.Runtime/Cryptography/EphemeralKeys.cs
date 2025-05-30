using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace BosonWare.Cryptography;

public static class EphemeralKeys
{
    private static readonly ConcurrentDictionary<string, byte[]> _ephemeralKeys = [];

    /// <summary>
    /// Gets an ephemeral key from the store with the specified <paramref name="keyName"/>. 
    /// If no key is found a new one is generated.
    /// </summary>
    /// <param name="keyName"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static byte[] Get(string keyName, int derivedKeyLength = 32)
    {
        if (!_ephemeralKeys.TryGetValue(keyName, out var ephemeralKey)) {
            ephemeralKey = GenerateKey(derivedKeyLength);

            _ephemeralKeys.TryAdd(keyName, ephemeralKey);
        }

        return ephemeralKey;
    }

    /// <summary>
    /// Generates a new ephemeral key.
    /// </summary>
    /// <param name="keyName"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static byte[] New(string keyName, int derivedKeyLength = 32)
    {
        byte[] ephemeralKey = GenerateKey(derivedKeyLength);

        _ephemeralKeys[keyName] = ephemeralKey;

        return ephemeralKey;
    }

    // Allocates 32 bytes of required heap memory and saves 32 bytes.
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    internal static byte[] GenerateKey(int derivedKeyLength)
    {
        Span<byte> salt = stackalloc byte[16];
        Span<byte> sugar = stackalloc byte[16]; // Pun intended

        Guid.NewGuid().TryWriteBytes(salt);
        Guid.NewGuid().TryWriteBytes(sugar);

        var ephemeralKey = KeyUtility.ComputeKey(salt, sugar, derivedKeyLength);

        return ephemeralKey;
    }

    // Allocates 64 bytes of heap memory.
    internal static byte[] GenerateKeyOld(int derivedKeyLength)
    {
        var salt = Guid.NewGuid().ToByteArray();
        var sugar = Guid.NewGuid().ToByteArray(); // Pun intended

        var ephemeralKey = KeyUtility.ComputeKey(salt, sugar, derivedKeyLength);

        return ephemeralKey;
    }
}