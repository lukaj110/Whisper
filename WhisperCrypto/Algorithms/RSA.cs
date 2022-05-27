using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Whisper.Crypto.Algorithms
{
    public class RSA4096
    {
        private readonly RSA rsa = RSA.Create(4096);

        public string PublicKey => Convert.ToBase64String(rsa.ExportSubjectPublicKeyInfo());

        public string PrivateKey => Convert.ToBase64String(rsa.ExportPkcs8PrivateKey());

        public void ImportPrivateKey(string privKey)
        {
            rsa.ImportPkcs8PrivateKey(Convert.FromBase64String(privKey), out _);
        }

        public void ImportPublicKey(string pubKey)
        {
            rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(pubKey), out _);
        }

        public void ExportKeys(string path) 
            => File.WriteAllText(Path.Combine(path, "Private.whisper"), PrivateKey);

        public void ImportKeys(string path)
            => ImportPrivateKey(File.ReadAllText(Path.Combine(path, "Private.whisper")));

        public string Encrypt(string message)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            byte[] rsaEncryptedMessage = rsa.Encrypt(messageBytes, RSAEncryptionPadding.Pkcs1);

            return Convert.ToBase64String(rsaEncryptedMessage);
        }

        public string Decrypt(string encryptedMessage)
        {
            byte[] messageBytes = Convert.FromBase64String(encryptedMessage);

            byte[] rsaDecryptedMessage = rsa.Decrypt(messageBytes, RSAEncryptionPadding.Pkcs1);

            return Encoding.UTF8.GetString(rsaDecryptedMessage);
        }

        public RSA4096() { }
    }
}