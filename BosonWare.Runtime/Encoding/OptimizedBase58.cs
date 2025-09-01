using UnmanagedMemory;

namespace BosonWare.Encoding;

/// <summary>
///     Memory-optimized Base58 encoder and decoder.
///     Uses unsafe memory operations for potentially better performance.
/// </summary>
[PublicAPI]
public static class OptimizedBase58
{
    /// <summary>
    ///     Memory-optimized Base58 encoding for a byte span.
    /// </summary>
    public static string EncodeData(ReadOnlySpan<byte> data)
    {
        return EncodeData(data, 0, data.Length);
    }

    /// <summary>
    ///     Memory-optimized Base58 encoding for a segment of a byte span.
    ///     Uses UnsafeMemory for better memory management.
    /// </summary>
    public static string EncodeData(ReadOnlySpan<byte> data, int offset, int count)
    {
        var zeroCount = 0;
        var length = 0;

        // Count leading zeros
        while (offset != count && data[offset] == 0) {
            offset++;
            zeroCount++;
        }

        // Approximate output size for Base58
        var size = (count - offset) * 138 / 100 + 1;

        using var bytes = new UnsafeMemory<byte>(size);

        // Encode bytes into Base58 representation
        while (offset != count) {
            int carry = data[offset];
            var i = 0;
            for (var j = size - 1; (carry != 0 || i < length) && j >= 0; j--, i++) {
                carry += 256 * bytes[j];
                bytes[j] = (byte)(carry % 58);
                carry /= 58;
            }
            length = i;
            offset++;
        }

        // Skip leading zeros in output
        var startIndex = size - length;
        while (startIndex < size && bytes[startIndex] == 0)
            startIndex++;

        using var chars = new UnsafeMemory<char>(zeroCount + size - startIndex);

        // Fill leading '1's for each zero byte
        chars.AsSpan(0, zeroCount).Fill('1');

        var k = zeroCount;
        while (startIndex < size) {
            chars[k++] = Base58.PszBase58[bytes[startIndex++]];
        }

        return new string(chars.AsSpan());
    }

    /// <summary>
    ///     Memory-optimized Base58 decoding from a span of encoded characters.
    /// </summary>
    public static byte[] DecodeData(ReadOnlySpan<char> encoded)
    {
        var i = 0;
        // Skip leading whitespace
        while (i < encoded.Length && DataEncoder.IsSpace(encoded[i])) i++;

        var zeroCount = 0;
        var length = 0;

        // Count leading '1's which represent zero bytes
        while (i < encoded.Length && encoded[i] == '1') {
            zeroCount++;
            i++;
        }

        var size = (encoded.Length - i) * 733 / 1000 + 1;
        using var bytes = new UnsafeMemory<byte>(size);

        // Decode Base58 characters into bytes
        while (i < encoded.Length && !DataEncoder.IsSpace(encoded[i])) {
            var carry = Base58.MapBase58[(byte)encoded[i]];
            if (carry == -1)
                throw new FormatException("Invalid base58 data");

            var j = 0;
            for (var k = size - 1; (carry != 0 || j < length) && k >= 0; k--, j++) {
                carry += 58 * bytes[k];
                bytes[k] = (byte)(carry % 256);
                carry /= 256;
            }
            length = j;
            i++;
        }

        // Skip trailing whitespace
        while (i < encoded.Length && DataEncoder.IsSpace(encoded[i])) i++;

        // Invalid data if not at end
        if (i != encoded.Length)
            throw new FormatException("Invalid base58 data");

        var startIndex = size - length;
        var data = new byte[zeroCount + size - startIndex];

        Array.Fill(data, (byte)0, 0, zeroCount);

        var pos = zeroCount;
        while (startIndex < size)
            data[pos++] = bytes[startIndex++];

        return data;
    }
}
