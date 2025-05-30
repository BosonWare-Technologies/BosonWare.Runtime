namespace BosonWare.Encoding;

public sealed class Base58Encoder : DataEncoder
{
    public override string EncodeData(byte[] data, int offset, int count)
    {
        return Base58.EncodeData(data, offset, count);
    }

    public override byte[] DecodeData(string encoded)
    {
        return Base58.DecodeData(encoded);
    }
}