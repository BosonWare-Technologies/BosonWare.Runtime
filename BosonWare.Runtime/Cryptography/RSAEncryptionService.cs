using System.Security.Cryptography;

namespace BosonWare.Cryptography;

public sealed class RSAEncryptionService : IEncryptionService
{
	private readonly RSA rsa;

	private RSAEncryptionService(RSA rsa) => this.rsa = rsa;

	public byte[] Encrypt(byte[] data) => rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);

	public byte[] Decrypt(byte[] data) => rsa.Decrypt(data, RSAEncryptionPadding.OaepSHA256);

	public string EncryptText(string text)
	{
		return Convert.ToBase64String(Encrypt(SystemEncoding.UTF8.GetBytes(text)));
	}

	public string DecryptText(string cipher)
	{
		return SystemEncoding.UTF8.GetString(Decrypt(Convert.FromBase64String(cipher)));
	}

	public void Dispose() => rsa.Dispose();

	public static RSAEncryptionService FromPemKey(ReadOnlySpan<char> pemKey)
	{
		var rsa = RSA.Create();

		rsa.ImportFromPem(pemKey);

		return new RSAEncryptionService(rsa);
	}
}