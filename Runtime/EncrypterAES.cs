using System.IO;
using System.Security.Cryptography;

namespace Saveable
{
    public static class EncrypterAES
    {
        /// GENERATE: https://www.random.org/cgi-bin/randbyte?nbytes=32%format=d
        private static readonly byte[] Key = { 45, 117, 29, 30, 159, 124, 0, 136, 129, 62, 126, 188, 52, 208, 58, 89, 139, 193, 179, 21, 186, 161, 45, 173, 2, 72, 11, 49, 129, 197, 5, 137 };

        private static readonly byte[] IV = { 200, 64, 191, 20, 9, 3, 5, 119, 231, 121, 252, 112, 79, 32, 114, 156 };

        public static byte[] EncryptStringToBytes_Aes(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                UnityEngine.Debug.LogError("Input was invalid!");
                return default;
            }

            byte[] encrypted;

            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using var msEncrypt = new MemoryStream();
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                        swEncrypt.Write(plainText);

                    encrypted = msEncrypt.ToArray();
                }
            }

            return encrypted;
        }

        public static string DecryptStringFromBytes_Aes(byte[] cipherText)
        {
            if (cipherText == null || cipherText.Length <= 0)
            {
                UnityEngine.Debug.LogError("Input was invalid!");
                return default;
            }

            string plainText;

            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using var msDecrypt = new MemoryStream(cipherText);
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (var srDecrypt = new StreamReader(csDecrypt))
                        plainText = srDecrypt.ReadToEnd();
                }
            }

            return plainText;
        }
    } 
}