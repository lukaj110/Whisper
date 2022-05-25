using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace WhisperCrypto.Algorithms
{
    internal class AES256
    {
        private readonly Aes aes;

        public AES256()
        {
            aes = Aes.Create();

            aes.BlockSize = 256;

            aes.KeySize = 256;

            aes.Mode = CipherMode.CBC;
        }

        public string Encrypt(string message, byte[] iv)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            byte[] aesEncryptedMessage = aes.EncryptCbc(messageBytes, iv);

            return Convert.ToBase64String(aesEncryptedMessage);
        }

        public string Decrypt(string encryptedMessage, byte[] iv)
        {
            byte[] messageBytes = Convert.FromBase64String(encryptedMessage);

            byte[] aesDecryptedMessage = aes.DecryptCbc(messageBytes, iv);

            return Encoding.UTF8.GetString(aesDecryptedMessage);
        }
    }
}
