using System.Security.Cryptography;
using System.Text;
using System;


namespace Bin2Reg.Encoders
{
    internal interface IEncoder
    {
        byte[] Encode(byte[] plain);
        byte[] Decode(byte[] cipher);
    }

    public class Dpapi
        : IEncoder
    {
        public DataProtectionScope ProtectionScope { get; set; }


        public Dpapi()
            : this(DataProtectionScope.LocalMachine)
        {
        }

        public Dpapi(DataProtectionScope scope)
        {
            ProtectionScope = scope;
        }

        public byte[] Encode(byte[] plain) => Encryption(ProtectedData.Protect, plain);

        public byte[] Decode(byte[] cipher) => Encryption(ProtectedData.Unprotect, cipher);

        private byte[] Encryption(Func<byte[], byte[], DataProtectionScope, byte[]> transform, byte[] data) => transform(data, null, ProtectionScope);
    }

    [Obsolete("Use 'Bin2Reg::Encoders::Dpapi' instead.", error:true)]
    public class XorEncoder
        : IEncoder
    {
        // Order alphabetically
        private const string Password = "Bart, Chris, Toby, Vicki";

        private byte[] Key { get; }


        public XorEncoder()
        {
            Key = Encoding.ASCII.GetBytes(Password);
        }

        public byte[] Encode(byte[] plain)
        {
            int klen = Key.Length;
            int plen = plain.Length;
            byte[] cipher = new byte[plen];

            for (int i = 0; i < plen; i++)
                cipher[i] = ByteXor(plain, klen, i);

            return cipher;
        }

        private byte ByteXor(byte[] plain, int keyLen, int counter) => (byte)(plain[counter] ^ Key[counter % keyLen]);

        public byte[] Decode(byte[] cipher) => Encode(cipher);
    }
}
