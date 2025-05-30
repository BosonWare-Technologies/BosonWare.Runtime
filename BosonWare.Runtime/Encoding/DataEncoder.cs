namespace BosonWare.Encoding;

public abstract class DataEncoder
{
    internal DataEncoder() { }

    public string EncodeData(byte[] data) => EncodeData(data, 0, data.Length);

    public abstract string EncodeData(byte[] data, int offset, int count);

    public abstract byte[] DecodeData(string encoded);

    public static bool IsSpace(char c) => c switch {
        '\t' or '\n' or '\v' or '\f' or '\r' or ' ' => true,
        _ => false,
    };
}