using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WhisperCrypto.Algorithms
{
    public static class PBKDF2
    {
        private const int SALT_SIZE = 64;
        private const int HASH_SIZE = 64;
        private const int ITERATIONS = 100000;

        private static string HashPassword(string password, string salt)
        {
            string[] pass = new string[2] 
            { password.Substring(0, password.Length / 2), 
                password.Substring(password.Length / 2, (int)Math.Ceiling((decimal)password.Length / 2)) };

            byte[] input = Encoding.UTF8.GetBytes(pass[0] + salt + pass[1]);

            Rfc2898DeriveBytes pbkdf = new(input, Encoding.UTF8.GetBytes(salt), ITERATIONS, HashAlgorithmName.SHA512);

            return Convert.ToBase64String(pbkdf.GetBytes(HASH_SIZE));
        }

        public static Tuple<string, string> HashNewPassword(string password)
        {
            string salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(SALT_SIZE));

            return new Tuple<string, string>(HashPassword(password, salt), salt);
        }

        public static bool Verify(string password, string salt, string hash) => HashPassword(password, salt) == hash;
    }
}
