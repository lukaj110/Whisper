using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Whisper.Crypto.Algorithms;

namespace Whisper.Client.Helpers
{
    public static class CryptoHelper
    {
        public static RSA4096 InitializeKeys()
        {
            var rsa = new RSA4096();

            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Whisper");

            var filePath = Path.Combine(path, "Private.whisper");

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            if (!File.Exists(filePath)) rsa.ExportKeys(path);

            else
            {
                rsa.ImportKeys(path);
            }

            return rsa;
        }
    }
}