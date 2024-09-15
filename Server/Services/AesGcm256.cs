using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System.Text;

namespace PasswordManager.Server.Services
{
    public class AesGcm256
    {
        private static readonly SecureRandom Random = new SecureRandom();

        public static readonly int NonceBitSize = 128;
        public static readonly int MacBitSize = 128;
        public static readonly int KeyBitSize = 256;

        private AesGcm256() { }

        public static byte[] GenerateNewKey()
        {
            var newKey = new byte[KeyBitSize / 8];
            Random.NextBytes(newKey);
            return newKey;
        }

        public static byte[] GenerateNewIv()
        {
            var newIv = new byte[NonceBitSize / 8];
            Random.NextBytes(newIv);
            return newIv;
        }

        public static byte[] ConvertHexToByte(string hexString)
        {
            byte[] byteArray = new byte[hexString.Length / 2];
            for (int i = 0; i < (hexString.Length / 2); i++)
            {
                byte firstNibble = Byte.Parse(hexString.Substring((2 * i), 1), System.Globalization.NumberStyles.HexNumber);
                byte secondNibble = Byte.Parse(hexString.Substring((2 * i) + 1, 1), System.Globalization.NumberStyles.HexNumber);
                int finalByte = (secondNibble) | (firstNibble << 4);
                byteArray[i] = (byte)finalByte;
            }
            return byteArray;
        }


        public static string ConvertToHex(byte[] data)
        {
            string hexString = string.Empty;
            foreach (byte b in data)
            {
                hexString += b.ToString("X2");
            }
            return hexString;
        }

        public static string ConvertToHex(string asciiString)
        {
            string hexString = string.Empty;
            foreach (char c in asciiString)
            {
                int tmp = c;
                hexString += string.Format("{0:x2}", System.Convert.ToUInt32(tmp.ToString()));
            }
            return hexString;
        }

        public static string Encrypt(string plainText, byte[] key, byte[] iv)
        {
            string encryptedResult = string.Empty;
            try
            {
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

                GcmBlockCipher cipher = new GcmBlockCipher(new AesEngine());
                AeadParameters parameters = new AeadParameters(new KeyParameter(key), 128, iv, null);

                cipher.Init(true, parameters);

                byte[] encryptedBytes = new byte[cipher.GetOutputSize(plainBytes.Length)];
                Int32 retLen = cipher.ProcessBytes(plainBytes, 0, plainBytes.Length, encryptedBytes, 0);
                cipher.DoFinal(encryptedBytes, retLen);
                encryptedResult = Convert.ToBase64String(encryptedBytes, Base64FormattingOptions.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            return encryptedResult;
        }


        public static string Decrypt(string encryptedText, byte[] key, byte[] iv)
        {
            string decryptedResult = string.Empty;
            try
            {
                byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

                GcmBlockCipher cipher = new GcmBlockCipher(new AesEngine());
                AeadParameters parameters = new AeadParameters(new KeyParameter(key), 128, iv, null);

                cipher.Init(false, parameters);
                byte[] plainBytes = new byte[cipher.GetOutputSize(encryptedBytes.Length)];
                Int32 retLen = cipher.ProcessBytes(encryptedBytes, 0, encryptedBytes.Length, plainBytes, 0);
                cipher.DoFinal(plainBytes, retLen);

                decryptedResult = Encoding.UTF8.GetString(plainBytes).TrimEnd("\r\n\0".ToCharArray());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            return decryptedResult;
        }
    }
}