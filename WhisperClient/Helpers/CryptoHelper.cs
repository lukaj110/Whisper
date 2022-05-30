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
        public static DiffieHellman InitializeKeys(string username)
        {
            var dh = new DiffieHellman();

            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Whisper");

            var filePath = Path.Combine(path, $"DH_{SHA512.Hash(username.ToUpper())}.whisper");

            if (!Directory.Exists(path)) return null;

            if (!File.Exists(filePath)) return null;

            else
            {
                dh.ImportKeys(filePath);
            }

            return dh;
        }
    }
}