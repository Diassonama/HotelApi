using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Hotel.Infrastruture.Services
{
    public class CrystoService
    {
        private const string Key = "MNBVCXZLKJ039RTY4U6W"; // Key must be securely stored
        private const string IV = "1234567890123456";      // Initialization Vector (must be 16 bytes for AES)

        // Generate a key using SHA256 (for a fixed 256-bit key)
        private static byte[] GenerateKey(string key)
        {
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
            }
        }

        // Encrypt a string using AES
        public static string Encrypt(string stringToEncrypt)
        {
            try
            {
                using (var aes = Aes.Create())
                {
                    aes.Key = GenerateKey(Key);
                    aes.IV = Encoding.UTF8.GetBytes(IV);
                    aes.Mode = CipherMode.CBC; // CBC is more secure than ECB
                    aes.Padding = PaddingMode.PKCS7;

                    var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                    var buffer = Encoding.UTF8.GetBytes(stringToEncrypt);

                    byte[] encrypted = encryptor.TransformFinalBlock(buffer, 0, buffer.Length);
                    return Convert.ToBase64String(encrypted);
                }
            }
            catch (Exception ex)
            {
                throw new CryptographicException("Encryption failed.", ex);
            }
        }

        // Decrypt a string using AES
        public static string Decrypt(string encryptedString)
        {
            try
            {
                using (var aes = Aes.Create())
                {
                    aes.Key = GenerateKey(Key);
                    aes.IV = Encoding.UTF8.GetBytes(IV);
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                    var buffer = Convert.FromBase64String(encryptedString);

                    byte[] decrypted = decryptor.TransformFinalBlock(buffer, 0, buffer.Length);
                    return Encoding.UTF8.GetString(decrypted);
                }
            }
            catch (Exception ex)
            {
                throw new CryptographicException("Decryption failed.", ex);
            }
        }
    }
}