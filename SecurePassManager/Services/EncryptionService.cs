using System.Security.Cryptography;

public class EncryptionService
{
    private const int Iterations = 100_000;
    private const int KeySize = 256;
    private const int BlockSize = 128;
    private const int SaltSize = 32;
    private readonly byte[] _salt;

    public EncryptionService()
    {
        _salt = GetOrCreateSalt();
    }

    private byte[] GetOrCreateSalt()
    {
        string saltPath = Path.Combine(FileSystem.AppDataDirectory, "encryption.salt");
        if (File.Exists(saltPath))
        {
            return File.ReadAllBytes(saltPath);
        }
        else
        {
            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            File.WriteAllBytes(saltPath, salt);
            return salt;
        }
    }

    private byte[] DeriveKey(string password)
    {
        using (var pbkdf2 = new Rfc2898DeriveBytes(password, _salt, Iterations, HashAlgorithmName.SHA256))
        {
            return pbkdf2.GetBytes(KeySize / 8);
        }
    }

    public string Encrypt(string plainText, string masterPassword)
    {
        byte[] encrypted;
        byte[] key = DeriveKey(masterPassword);

        using (Aes aes = Aes.Create())
        {
            aes.KeySize = KeySize;
            aes.BlockSize = BlockSize;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.GenerateIV();

            using (var encryptor = aes.CreateEncryptor(key, aes.IV))
            using (var ms = new MemoryStream())
            {
                ms.Write(aes.IV, 0, aes.IV.Length); // Prepend IV to ciphertext
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                }
                encrypted = ms.ToArray();
            }
        }
        return Convert.ToBase64String(encrypted);
    }

    public string Decrypt(string cipherText, string masterPassword)
    {
        byte[] buffer = Convert.FromBase64String(cipherText);
        byte[] key = DeriveKey(masterPassword);

        using (Aes aes = Aes.Create())
        {
            aes.KeySize = KeySize;
            aes.BlockSize = BlockSize;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            var iv = new byte[aes.BlockSize / 8];
            Array.Copy(buffer, iv, iv.Length);

            using (var decryptor = aes.CreateDecryptor(key, iv))
            using (var ms = new MemoryStream(buffer, iv.Length, buffer.Length - iv.Length))
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (var sr = new StreamReader(cs))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
