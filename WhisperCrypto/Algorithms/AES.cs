using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Whisper.Crypto.Algorithms
{
    public class AES256
    {
        private readonly Aes aes;

        public AES256(byte[] key)
        {
            aes = Aes.Create();

            aes.BlockSize = 128;

            aes.KeySize = 256;

            aes.Mode = CipherMode.CBC;

            aes.Key = key;
        }

        public string EncryptEcb(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            byte[] aesEncryptedMessage = aes.EncryptEcb(messageBytes, PaddingMode.PKCS7);

            return Convert.ToBase64String(aesEncryptedMessage);
        }

        public string DecryptEcb(string encryptedMessage)
        {
            byte[] messageBytes = Convert.FromBase64String(encryptedMessage);

            byte[] aesDecryptedMessage = aes.DecryptEcb(messageBytes, PaddingMode.PKCS7);

            return Encoding.UTF8.GetString(aesDecryptedMessage);
        }

        public string EncryptCbc(string message, byte[] iv)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            byte[] aesEncryptedMessage = aes.EncryptCbc(messageBytes, iv);

            return Convert.ToBase64String(aesEncryptedMessage);
        }

        public string DecryptCbc(string encryptedMessage, byte[] iv)
        {
            byte[] messageBytes = Convert.FromBase64String(encryptedMessage);

            byte[] aesDecryptedMessage = aes.DecryptCbc(messageBytes, iv);

            return Encoding.UTF8.GetString(aesDecryptedMessage);
        }
    }
}
