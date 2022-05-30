using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Whisper.Crypto.Algorithms
{
    public static class SHA512
    {
        public static string Hash(string input)
        {
            var crypt = System.Security.Cryptography.SHA512.Create();

            string hash = string.Empty;

            byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(input));

            foreach (byte theByte in crypto)
            {
                hash += theByte.ToString("x2");
            }

            Debug.WriteLine(hash);

            return hash;
        }
    }
}