using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Whisper.Crypto.Algorithms;
using System.Security.Cryptography;

namespace Whisper.Client.Helpers
{
    public static class CryptoHelper
{
        public static DiffieHellman InitializeKeys()
        {
            var dh = new DiffieHellman();

            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Whisper");

            var filePath = Path.Combine(path, "DH.whisper");

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            if (!File.Exists(filePath)) dh.ExportKeys(path);

            else
            {
                dh.ImportKeys(path);
            }

            return dh;
        }
    }
}