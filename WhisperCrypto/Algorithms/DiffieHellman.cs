using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Whisper.Crypto.Algorithms
{
    [SupportedOSPlatform("windows")]
    public class DiffieHellman
    {
        private readonly ECDiffieHellmanCng diffieHellman = new()
        {
            KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash,
            HashAlgorithm = CngAlgorithm.Sha512
        };

        public string PublicKey => Convert.ToBase64String(diffieHellman.PublicKey.ToByteArray());

        private string PrivateKey => Convert.ToBase64String(diffieHellman.ExportPkcs8PrivateKey());

        public void ImportKeyPair(string keyPair)
        {
            diffieHellman.ImportPkcs8PrivateKey(Convert.FromBase64String(keyPair), out _);
        }

        public void ExportKeys(string path)
            => File.WriteAllText(path, PrivateKey);

        public void ImportKeys(string path)
            => ImportKeyPair(File.ReadAllText(path));

        public void ExportNewKeys(string username)
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Whisper");

            var filePath = Path.Combine(path, $"DH_{SHA512.Hash(username.ToUpper())}.whisper");

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            if (!File.Exists(filePath)) ExportKeys(filePath);
        }

        public byte[] DeriveKey(byte[] otherKey)
        {
            return diffieHellman.DeriveKeyMaterial(CngKey.Import(otherKey, CngKeyBlobFormat.EccPublicBlob));
        }

        public DiffieHellman() { }
    }
}
