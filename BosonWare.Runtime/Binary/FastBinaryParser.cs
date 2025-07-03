namespace BosonWare.Binary;

/// <summary>
/// Provides methods for parsing binary data into integer and floating-point representations.
/// </summary>
public static class FastBinaryParser
{
    public enum ResultType
    {
        Invalid,
        Int16,
        Int32,
        Int64,
        Single,
        Double
    }

    public readonly ref struct Result
    {
        public ResultType Type { get; }

        public ReadOnlySpan<byte> Value { get; }

        public Result(ResultType type, ReadOnlySpan<byte> value)
        {
            Type = type;
            Value = value;
        }

        public short AsInt16() => BitConverter.ToInt16(Value);

        public int AsInt32() => BitConverter.ToInt32(Value);

        public long AsInt64() => BitConverter.ToInt64(Value);

        public float AsSingle() => BitConverter.ToSingle(Value);

        public double AsDouble() => BitConverter.ToDouble(Value);
    }

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
    public static bool TryParseInteger(ReadOnlySpan<byte> bytes, out Result value)
    {
        ResultType type;
        if (bytes.Length == 2) { // Check if the bytes array encodes a 16 bit integer.
            type = ResultType.Int16;
        }
        else if (bytes.Length == 4) { // Check if the bytes array encodes a 32 bit integer.
            type = ResultType.Int32;
        }
        else if (bytes.Length == 8) {  // Check if the bytes array encodes a 64 bit integer.
            type = ResultType.Int64;
        }
        else {
            type = ResultType.Invalid;
        }

        value = new Result(ResultType.Int16, bytes);

        return type != ResultType.Invalid;
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
    public static bool TryParseFloat(ReadOnlySpan<byte> bytes, out Result value)
    {
        ResultType type;
        if (bytes.Length == 4) // Check if the bytes array encodes a 16 bit integer.
        {
            type = ResultType.Single;
        }
        else if (bytes.Length == 8) // Check if the bytes array encodes a 32 bit integer.
        {
            type = ResultType.Double;
        }
        else {
            type = ResultType.Invalid;
        }

        value = new Result(ResultType.Int16, bytes);

        return type != ResultType.Invalid;
    }
}
