namespace BosonWare.Cryptography;

public interface IEncryptionService : IDisposable
{
	byte[] Encrypt(byte[] data);


	byte[] Decrypt(byte[] data);

	string EncryptText(string text);

	string DecryptText(string cipher);
}