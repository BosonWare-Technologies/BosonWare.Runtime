using System.Security.Cryptography;

namespace BosonWare.Cryptography;

public sealed class RSAEncryptionService : IEncryptionService, IDisposable
{
	private readonly RSA rsa;

	private RSAEncryptionService(RSA rsa) => this.rsa = rsa;

	public byte[] Encrypt(byte[] data)
	{
		return rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
	}

	public byte[] Decrypt(byte[] data)
	{
		return rsa.Decrypt(data, RSAEncryptionPadding.OaepSHA256);
	}

	public void Dispose() => rsa.Dispose();

	public static RSAEncryptionService FromPemKey(ReadOnlySpan<char> pemKey)
	{
		var rsa = RSA.Create();

		rsa.ImportFromPem(pemKey);

		return new RSAEncryptionService(rsa);
	}
}