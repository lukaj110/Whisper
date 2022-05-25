using System.Diagnostics;
using Whisper.Crypto.Algorithms;

namespace Whisper.Test
{
    [TestClass]
    public class Crypto
    {
        [TestMethod]
        public void PbkdfNewHashTest()
        {
            string pass = "testing password hashing using pbkdf2";

            Tuple<string, string> pass_salt = PBKDF2.HashNewPassword(pass);

            Console.WriteLine($"Password: {pass_salt.Item1}\nSalt: {pass_salt.Item2}");

            string hashedPass = PBKDF2.HashPassword(pass, pass_salt.Item2);

            Console.WriteLine($"Password: {hashedPass}");

            Assert.AreEqual(pass_salt.Item1, hashedPass);

            Assert.IsTrue(PBKDF2.Verify(pass, pass_salt.Item2, hashedPass));
        }

        [TestMethod]
        public void RSATest()
        {
            /* Situation
             * 
             * User 1 and User 2 want to communicate. They have their own public/private key pair.
             *
             * Both of them send their public keys to a server.
             * The server stores the public keys, and then distributes it to users that want to send the person messages.
             *
             * User 1 has the public key of User 2 to send them messages.
             * User 2 has the public key of User 1 to send them messages.
             *
             */

            RSA4096 user1 = new();
            RSA4096 user2 = new();

            Stopwatch stopwatch = Stopwatch.StartNew();

            string user1PublicKey = user1.PublicKey;
            string user1PrivateKey = user1.PrivateKey;
            string user2PublicKey = user2.PublicKey;
            string user2PrivateKey = user2.PrivateKey;

            Console.WriteLine($"Person 1 Public Key: {user1PublicKey}");
            Console.WriteLine($"Person 1 Private Key: {user1PrivateKey}");

            Console.WriteLine($"Person 2 Public Key: {user2PublicKey}");
            Console.WriteLine($"Person 2 Private Key: {user2PrivateKey}");
                        
            Console.WriteLine($"Time to import 2 Keys: {stopwatch.ElapsedMilliseconds} ms");


            RSA4096 user1_user2 = new();
            RSA4096 user2_user1 = new();

            user1_user2.ImportPublicKey(user2PublicKey);

            user2_user1.ImportPublicKey(user1PublicKey);

            Assert.AreEqual(user1PublicKey, user2_user1.PublicKey);

            Assert.AreEqual(user2PublicKey, user1_user2.PublicKey);

            string person1Message = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer lobortis metus in ligula facilisis, sed placerat diam tristique. Etiam maximus diam sit amet sem molestie, a dictum risus rutrum. Duis efficitur, ipsum a sagittis malesuada, sem est maximus eros, quis aliquet metus neque id sem. Praesent et ultrices tellus. Proin commodo augue vitae aliquam aliquam. Nulla tortor quam, consequat id consectetur in, eleifend a magna. Nulla malesuada odio nulla, at posuere dui egestas vitae erat curae.";

            string person1EncryptedMessage = user1_user2.Encrypt(person1Message);

            Console.WriteLine($"Encrypted message: {person1EncryptedMessage}");

            string person1DecryptedMessage = user2.Decrypt(person1EncryptedMessage);

            Console.WriteLine(person1DecryptedMessage);

            Assert.AreEqual(person1Message, person1DecryptedMessage);
        }
    }
}