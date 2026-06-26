using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BimNinja.Licensing
{
    public static class CryptoHelper
    {
        public static string Encrypt(string plainText, string password)
        {
            byte[] salt = GenerateSalt();
            using (var aes = Aes.Create())
            {
                var key = DeriveKey(password, salt);
                aes.Key = key;
                aes.GenerateIV();
                using (var encryptor = aes.CreateEncryptor())
                using (var ms = new MemoryStream())
                {
                    ms.Write(salt, 0, salt.Length);
                    ms.Write(aes.IV, 0, aes.IV.Length);
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        public static string Decrypt(string cipherText, string password)
        {
            byte[] fullCipher = Convert.FromBase64String(cipherText);
            byte[] salt = new byte[16];
            byte[] iv = new byte[16];
            Array.Copy(fullCipher, 0, salt, 0, 16);
            Array.Copy(fullCipher, 16, iv, 0, 16);
            byte[] cipher = new byte[fullCipher.Length - 32];
            Array.Copy(fullCipher, 32, cipher, 0, cipher.Length);

            using (var aes = Aes.Create())
            {
                aes.Key = DeriveKey(password, salt);
                aes.IV = iv;
                using (var decryptor = aes.CreateDecryptor())
                using (var ms = new MemoryStream(cipher))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        private static byte[] DeriveKey(string password, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
            {
                return pbkdf2.GetBytes(32);
            }
        }

        private static byte[] GenerateSalt()
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(salt);
            return salt;
        }
    }
}
