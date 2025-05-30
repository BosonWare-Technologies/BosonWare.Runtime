using System.Diagnostics.CodeAnalysis;

namespace BosonWare.Binary;

public static class BinaryParser
{
    public static bool TryParseInteger(ReadOnlySpan<byte> bytes, [NotNullWhen(true)] out string? type, [NotNullWhen(true)] out string? value)
    {
        if (bytes.Length == 2) { // Check if the bytes array encodes a 16 bit integer.
            type = "int16";

            value = BitConverter.ToInt16(bytes).ToString();

            return true;
        }
        else if (bytes.Length == 4) { // Check if the bytes array encodes a 32 bit integer.
            type = "int32";

            value = BitConverter.ToInt32(bytes).ToString();

            return true;
        }
        else if (bytes.Length == 8) {  // Check if the bytes array encodes a 64 bit integer.
            type = "int64";

            value = BitConverter.ToInt64(bytes).ToString();

            return true;
        }

        type = value = null;

        return false;
    }

    public static bool TryParseFloat(ReadOnlySpan<byte> bytes, [NotNullWhen(true)] out string? type, [NotNullWhen(true)] out string? value)
    {
        if (bytes.Length == 4) { // Check if the bytes array encodes a 32 bit floating point number.
            type = "single";

            value = BitConverter.ToSingle(bytes).ToString();

            return true;
        }
        else if (bytes.Length == 8) { // Check if the bytes array encodes a 64 bit floating point number.
            type = "double";

            value = BitConverter.ToDouble(bytes).ToString();

            return true;
        }

        type = value = null;

        return false;
    }

}