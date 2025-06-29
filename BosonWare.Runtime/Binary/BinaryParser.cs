using System.Diagnostics.CodeAnalysis;

namespace BosonWare.Binary;

/// <summary>
/// Provides methods for parsing binary data into integer and floating-point representations.
/// </summary>
public static class BinaryParser
{
    /// <summary>
    /// Attempts to parse a span of bytes as a signed integer (16, 32, or 64 bits).
    /// </summary>
    /// <param name="bytes">The span of bytes to parse.</param>
    /// <param name="type">
    /// When this method returns <c>true</c>, contains the type of integer parsed ("int16", "int32", or "int64").
    /// Otherwise, <c>null</c>.
    /// </param>
    /// <param name="value">
    /// When this method returns <c>true</c>, contains the string representation of the parsed integer value.
    /// Otherwise, <c>null</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the bytes could be parsed as a supported integer type; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryParseInteger(ReadOnlySpan<byte> bytes, [NotNullWhen(true)] out string? type, [NotNullWhen(true)] out string? value)
    {
        if (bytes.Length == 2)
        { // Check if the bytes array encodes a 16 bit integer.
            type = "int16";

            value = BitConverter.ToInt16(bytes).ToString();

            return true;
        }
        else if (bytes.Length == 4)
        { // Check if the bytes array encodes a 32 bit integer.
            type = "int32";

            value = BitConverter.ToInt32(bytes).ToString();

            return true;
        }
        else if (bytes.Length == 8)
        {  // Check if the bytes array encodes a 64 bit integer.
            type = "int64";

            value = BitConverter.ToInt64(bytes).ToString();

            return true;
        }

        type = value = null;

        return false;
    }

    /// <summary>
    /// Attempts to parse a span of bytes as a floating-point number (single or double precision).
    /// </summary>
    /// <param name="bytes">The span of bytes to parse.</param>
    /// <param name="type">
    /// When this method returns <c>true</c>, contains the type of floating-point number parsed ("single" or "double").
    /// Otherwise, <c>null</c>.
    /// </param>
    /// <param name="value">
    /// When this method returns <c>true</c>, contains the string representation of the parsed floating-point value.
    /// Otherwise, <c>null</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the bytes could be parsed as a supported floating-point type; otherwise, <c>false</c>.
    /// </returns>
    public static bool TryParseFloat(ReadOnlySpan<byte> bytes, [NotNullWhen(true)] out string? type, [NotNullWhen(true)] out string? value)
    {
        if (bytes.Length == 4)
        { // Check if the bytes array encodes a 32 bit floating point number.
            type = "single";

            value = BitConverter.ToSingle(bytes).ToString();

            return true;
        }
        else if (bytes.Length == 8)
        { // Check if the bytes array encodes a 64 bit floating point number.
            type = "double";

            value = BitConverter.ToDouble(bytes).ToString();

            return true;
        }

        type = value = null;

        return false;
    }

}