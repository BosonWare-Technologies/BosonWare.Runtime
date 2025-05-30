namespace BosonWare.Cryptography;

public interface IEncryptionService
{
	byte[] Encrypt(byte[] data);

	byte[] Decrypt(byte[] data);
}