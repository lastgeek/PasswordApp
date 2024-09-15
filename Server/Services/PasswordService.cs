using System.Security.Cryptography;
using System.Text;

namespace PasswordManager.Server.Services;

public class PasswordService
{
    public string EncryptPassword(string plainText, string salt)
    {
        if(string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException(nameof(plainText));
        if (string.IsNullOrEmpty(salt))
            throw new ArgumentNullException(nameof(salt));

        var key = ByteArrayToHexString(GenerateKey(salt));
        var iv = ByteArrayToHexString(GenerateIV(salt));

        string encryptedText = AesGcm256.Encrypt(plainText, AesGcm256.ConvertHexToByte(key), AesGcm256.ConvertHexToByte(iv));

        return encryptedText;
    }
    public static string ByteArrayToHexString(byte[] bytes)
    {
        StringBuilder hex = new StringBuilder(bytes.Length * 2);
        foreach (byte b in bytes)
        {
            hex.AppendFormat("{0:X2}", b);
        }
        return hex.ToString();
    }
    public byte[] GenerateKey(string text)
    {
        byte[] textBytes = Encoding.UTF8.GetBytes(text);
        byte[] key;

        using (SHA512 sha512 = SHA512.Create())
        {
            byte[] hash = sha512.ComputeHash(textBytes);
            key = new byte[32];
            Array.Copy(hash, key, key.Length);
        }
        return key;
    }
    public byte[] GenerateIV(string text)
    {
        byte[] textBytes = Encoding.UTF8.GetBytes(text);
        byte[] iv;

        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hash = sha256.ComputeHash(textBytes);
            iv = new byte[16];
            Array.Copy(hash, iv, iv.Length);
        }

        return iv;
    }
    public string DecryptPassword(string plainText, string salt)
    {
        if (string.IsNullOrEmpty(plainText))
            throw new ArgumentNullException(nameof(plainText));
        if (string.IsNullOrEmpty(salt))
            throw new ArgumentNullException(nameof(salt));

        try
        {
            var key = ByteArrayToHexString(GenerateKey(salt));
            var iv = ByteArrayToHexString(GenerateIV(salt));

            string decryptedText = AesGcm256.Decrypt(plainText, AesGcm256.ConvertHexToByte(key), AesGcm256.ConvertHexToByte(iv));

            return decryptedText;
        }
        catch (Exception ex)
        {
            throw new Exception("Failed to decrypt password", ex);
        }
    }
}