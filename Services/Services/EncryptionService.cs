using Microsoft.Extensions.Configuration;
using Services.Interfaces;
using System.Security.Cryptography;

public class EncryptionService : IEncryptionService
{
    private readonly byte[] _key;
    public EncryptionService(IConfiguration configuration)
    {
        var secretKey = configuration["Encryption:SecretKey"];
        if (string.IsNullOrEmpty(secretKey) || secretKey.Length != 32)
        {
            throw new ArgumentException("The encryption key (Encryption:SecretKey) must be 32 characters long.");
        }
        _key = System.Text.Encoding.UTF8.GetBytes(secretKey);
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return plainText;

        using (var aes = Aes.Create())
        {
            aes.Key = _key;
            aes.GenerateIV();
            var iv = aes.IV;

            using (var encryptor = aes.CreateEncryptor(aes.Key, iv))
            using (var ms = new MemoryStream())
            {
                ms.Write(iv, 0, iv.Length);
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                }
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }

    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            return cipherText;

        var fullCipher = Convert.FromBase64String(cipherText);

        using (var aes = Aes.Create())
        {
            var iv = new byte[aes.BlockSize / 8];
            Array.Copy(fullCipher, 0, iv, 0, iv.Length);

            aes.Key = _key;
            aes.IV = iv;

            using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
            using (var ms = new MemoryStream(fullCipher, iv.Length, fullCipher.Length - iv.Length))
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (var sr = new StreamReader(cs))
            {
                return sr.ReadToEnd();
            }
        }
    }
}