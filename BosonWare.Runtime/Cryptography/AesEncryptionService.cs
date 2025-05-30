using System.Security.Cryptography;

namespace BosonWare.Cryptography;

public sealed class AesEncryptionService : IEncryptionService, IDisposable
{
	private readonly Aes _aes;

	private readonly byte[] _rgbKey;

	public AesEncryptionService(byte[] rgbKey)
	{
		_rgbKey = rgbKey;
		_aes = Aes.Create();

		_aes.KeySize = 256;
		_aes.Mode = CipherMode.CBC;
		_aes.Padding = PaddingMode.PKCS7;
	}

	public byte[] Encrypt(byte[] data)
	{
		// Encrypt the concatenated byte array
		using var encryptor = _aes.CreateEncryptor(_rgbKey, _aes.IV);
		using var memoryStream = new MemoryStream();

		// Write IV to the memory stream
		memoryStream.Write(_aes.IV, 0, _aes.IV.Length);

		using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write)) {
			cryptoStream.Write(data, 0, data.Length);
			cryptoStream.FlushFinalBlock();
		}

		return memoryStream.ToArray();
	}

	public byte[] Decrypt(byte[] data)
	{
		using var memoryStream = new MemoryStream(data);

		// Read IV from the encrypted data
		var iv = new byte[_aes.IV.Length];

		memoryStream.Read(iv, 0, iv.Length);

		using var decryptor = _aes.CreateDecryptor(_rgbKey, iv);
		using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
		using var resultStream = new MemoryStream();

		cryptoStream.CopyTo(resultStream);

		byte[] decryptedBytes = resultStream.ToArray();

		return decryptedBytes;
	}

	public void Dispose() => _aes.Dispose();
}