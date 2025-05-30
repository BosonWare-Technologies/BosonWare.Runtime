namespace BosonWare.Encoding;

public static class Base58
{
    // The base58 characters.
    private static readonly char[] PszBase58 = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz".ToCharArray();

    private static readonly Dictionary<char, bool> Validator = PszBase58.ToDictionary((x) => x, (x) => true);

    private static readonly int[] MapBase58 =
    [
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

    public static string EncodeData(ReadOnlySpan<byte> data) => EncodeData(data, 0, data.Length);

    public static string EncodeData(ReadOnlySpan<byte> data, int offset, int count)
    {
        int num = 0;
        int num2 = 0;
        while (offset != count && data[offset] == 0) {
            offset++;
            num++;
        }

        int num3 = (count - offset) * 138 / 100 + 1;
        byte[] array = new byte[num3];

        while (offset != count) {
            int num4 = data[offset];
            int num5 = 0;
            int num6 = num3 - 1;
            while ((num4 != 0 || num5 < num2) && num6 >= 0) {
                num4 += 256 * array[num6];
                array[num6] = (byte)(num4 % 58);
                num4 /= 58;
                num5++;
                num6--;
            }

            num2 = num5;
            offset++;
        }

        int i;
        for (i = num3 - num2; i != num3 && array[i] == 0; i++) { }

        char[] array2 = new char[num + num3 - i];

        Array.Fill(array2, '1', 0, num);

        int num7 = num;

        while (i != num3) {
            array2[num7++] = PszBase58[array[i++]];
        }

        return new string(array2);
    }

    public static byte[] DecodeData(ReadOnlySpan<char> encoded)
    {
        int i;
        for (i = 0; i < encoded.Length && DataEncoder.IsSpace(encoded[i]); i++) { }

        int num = 0;
        int num2 = 0;
        for (; i < encoded.Length && encoded[i] == '1'; i++) {
            num++;
        }

        int num3 = (encoded.Length - i) * 733 / 1000 + 1;
        byte[] array = new byte[num3];
        for (; i < encoded.Length && !DataEncoder.IsSpace(encoded[i]); i++) {
            int num4 = MapBase58[(byte)encoded[i]];
            if (num4 == -1) {
                throw new FormatException("Invalid base58 data");
            }

            int num5 = 0;
            int num6 = num3 - 1;
            while ((num4 != 0 || num5 < num2) && num6 >= 0) {
                num4 += 58 * array[num6];
                array[num6] = (byte)(num4 % 256);
                num4 /= 256;
                num5++;
                num6--;
            }

            num2 = num5;
        }

        for (; i < encoded.Length && DataEncoder.IsSpace(encoded[i]); i++) { }

        if (i != encoded.Length) {
            throw new FormatException("Invalid base58 data");
        }

        int num7 = num3 - num2;
        byte[] array2 = new byte[num + num3 - num7];

        Array.Fill(array2, (byte)0, 0, num);

        int num8 = num;

        while (num7 != num3) {
            array2[num8++] = array[num7++];
        }

        return array2;
    }

    public static bool IsValidWithoutWhitespace(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        for (int i = 0; i < value.Length; i++) {
            if (!Validator.ContainsKey(value[i])) {
                return false;
            }
        }

        return true;
    }
}