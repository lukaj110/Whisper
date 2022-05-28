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
            => File.WriteAllText(Path.Combine(path, "DH.whisper"), PrivateKey);

        public void ImportKeys(string path)
            => ImportKeyPair(File.ReadAllText(Path.Combine(path, "DH.whisper")));

        public byte[] DeriveKey(byte[] otherKey)
        {
            return diffieHellman.DeriveKeyMaterial(CngKey.Import(otherKey, CngKeyBlobFormat.EccPublicBlob));
        }

        public DiffieHellman() { }
    }
}
