namespace BosonWare.Encoding;

/// <summary>
///     Base58 encoder and decoder.
/// </summary>
[PublicAPI]
public static class Base58
{
    // Base58 character set used for encoding/decoding
    internal static readonly char[] PszBase58 = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz".ToCharArray();

    // Validator dictionary for quick character validation
    internal static readonly Dictionary<char, bool> Validator = PszBase58.ToDictionary(x => x, x => true);

    // Mapping from ASCII byte to Base58 index, -1 means invalid character for decoding
    internal static readonly int[] MapBase58 = [
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, 0,
        1, 2, 3, 4, 5, 6, 7, 8, -1, -1,
        -1, -1, -1, -1, -1, 9, 10, 11, 12, 13,
        14, 15, 16, -1, 17, 18, 19, 20, 21, -1,
        22, 23, 24, 25, 26, 27, 28, 29, 30, 31,
        32, -1, -1, -1, -1, -1, -1, 33, 34, 35,
        36, 37, 38, 39, 40, 41, 42, 43, -1, 44,
        45, 46, 47, 48, 49, 50, 51, 52, 53, 54,
        55, 56, 57, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
        -1, -1, -1, -1, -1, -1
    ];

    /// <summary>
    ///     Encodes a byte span into a Base58 string.
    /// </summary>
    public static string EncodeData(ReadOnlySpan<byte> data)
    {
        return EncodeData(data, 0, data.Length);
    }

    /// <summary>
    ///     Encodes a segment of a byte span into Base58 string.
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

        // Approximate size for base58 representation
        var size = (count - offset) * 138 / 100 + 1;
        var buffer = new byte[size];

        // Convert bytes to Base58
        while (offset != count) {
            int carry = data[offset];
            var i = 0;
            for (var j = size - 1; (carry != 0 || i < length) && j >= 0; j--, i++) {
                carry += 256 * buffer[j];
                buffer[j] = (byte)(carry % 58);
                carry /= 58;
            }
            length = i;
            offset++;
        }

        // Skip leading zeros in buffer
        var startIndex = size - length;
        while (startIndex < size && buffer[startIndex] == 0)
            startIndex++;

        // Prepare output char array with leading '1's for each leading zero byte
        var result = new char[zeroCount + size - startIndex];
        Array.Fill(result, '1', 0, zeroCount);

        var k = zeroCount;
        while (startIndex < size) {
            result[k++] = PszBase58[buffer[startIndex++]];
        }

        return new string(result);
    }

    /// <summary>
    ///     Decodes a Base58-encoded span of characters to a byte array.
    /// </summary>
    public static byte[] DecodeData(ReadOnlySpan<char> encoded)
    {
        var i = 0;
        // Skip leading whitespaces
        while (i < encoded.Length && DataEncoder.IsSpace(encoded[i])) i++;

        var zeroCount = 0;
        var length = 0;

        // Count leading '1's which represent zero bytes
        while (i < encoded.Length && encoded[i] == '1') {
            zeroCount++;
            i++;
        }

        var size = (encoded.Length - i) * 733 / 1000 + 1;
        var buffer = new byte[size];

        // Decode Base58 characters to bytes
        while (i < encoded.Length && !DataEncoder.IsSpace(encoded[i])) {
            var carry = MapBase58[(byte)encoded[i]];
            if (carry == -1) throw new FormatException("Invalid base58 data");

            var j = 0;
            for (var k = size - 1; (carry != 0 || j < length) && k >= 0; k--, j++) {
                carry += 58 * buffer[k];
                buffer[k] = (byte)(carry % 256);
                carry /= 256;
            }
            length = j;
            i++;
        }

        // Skip trailing whitespaces
        while (i < encoded.Length && DataEncoder.IsSpace(encoded[i])) i++;

        // If not at end, invalid input detected
        if (i != encoded.Length) throw new FormatException("Invalid base58 data");

        var startIndex = size - length;
        var result = new byte[zeroCount + size - startIndex];
        Array.Fill(result, (byte)0, 0, zeroCount);

        var pos = zeroCount;
        while (startIndex < size)
            result[pos++] = buffer[startIndex++];

        return result;
    }

    /// <summary>
    ///     Validates a Base58 string assuming no whitespace present.
    /// </summary>
    public static bool IsValidWithoutWhitespace(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        foreach (var ch in value.AsSpan()) {
            if (!Validator.ContainsKey(ch))
                return false;
        }

        return true;
    }
}
