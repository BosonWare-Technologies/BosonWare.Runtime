using System.Runtime.CompilerServices;

namespace BosonWare.Binary;

/// <summary>
/// Provides methods for parsing binary data into integer and floating-point representations.
/// </summary>
public static class FastBinaryParser
{
    public enum ResultType
    {
        Invalid,
        Int8,
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte AsInt8() => Value[0];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short AsInt16() => BitConverter.ToInt16(Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int AsInt32() => BitConverter.ToInt32(Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long AsInt64() => BitConverter.ToInt64(Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float AsSingle() => BitConverter.ToSingle(Value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double AsDouble() => BitConverter.ToDouble(Value);
    }

    /// <summary>
    /// Attempts to parse a span of bytes as a signed integer (8, 16, 32, or 64 bits).
    /// </summary>
    /// <param name="bytes">The span of bytes to parse.</param>
    /// <param name="value">The result</param>
    /// <returns>
    /// <c>true</c> if the bytes could be parsed as a 
    /// supported integer type; otherwise, <c>false</c>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseInteger(ReadOnlySpan<byte> bytes, out Result value)
    {
        var type = bytes.Length switch {
            1 => ResultType.Int8,
            2 => ResultType.Int16,
            4 => ResultType.Int32,
            8 => ResultType.Int64,
            _ => ResultType.Invalid,
        };

        value = new Result(ResultType.Int16, bytes);

        return type != ResultType.Invalid;
    }

    /// <summary>
    /// Attempts to parse a span of bytes as a floating-point number (single or double precision).
    /// </summary>
    /// <param name="bytes">The span of bytes to parse.</param>
    /// <param name="value">The result</param>
    /// <returns>
    /// <c>true</c> if the bytes could be parsed as a 
    /// supported floating-point type; otherwise, <c>false</c>.
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseFloat(ReadOnlySpan<byte> bytes, out Result value)
    {
        var type = bytes.Length switch {
            4 => ResultType.Single,
            8 => ResultType.Double,
            _ => ResultType.Invalid,
        };

        value = new Result(ResultType.Int16, bytes);

        return type != ResultType.Invalid;
    }
}
