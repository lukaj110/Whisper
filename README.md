# Whisper

Whisper is a centralized chatting application that achieves the 3 main goals of cryptography: confidentiality, integrity, and authenticity.

## How it works

Firstly, the user needs to register. During the registration process, a [Diffie-Hellman Keypair](https://en.wikipedia.org/wiki/Diffie%E2%80%93Hellman_key_exchange)  is created in the background and saved. The public portion of the key is sent to the server, along with the user credentials, which are hashed using a [Key Derivation Function](https://en.wikipedia.org/wiki/PBKDF2).

Once the user is registered/logged in, they receive a [JSON Web Token](https://jwt.io/introduction) which is used for API Authentication.

### One on One chat

Users can add other users and send them messages. By sending someone a message, the recipient's Public Key gets fetched from the server, and it gets combined with the user's own Diffie-Hellman Keypair to form a new unique key which is used in pair with [AES](https://en.wikipedia.org/wiki/Advanced_Encryption_Standard) to encrypt message data.

### Group Chat

To do
