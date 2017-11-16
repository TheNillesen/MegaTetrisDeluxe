using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Intermediate
{
    public class Encryption
    {
        private RSACryptoServiceProvider rsa;

        public Encryption()
        {
            rsa = new RSACryptoServiceProvider();
        }

        public Encryption(byte[] key)
        {
            byte[] actualKey = BitFlip(key);

            rsa = new RSACryptoServiceProvider();
            rsa.ImportCspBlob(actualKey);
        }

        public byte[] GetPublicKey()
        {
            return BitFlip(rsa.ExportCspBlob(false));
        }

        public void FromKey(byte[] key)
        {
            byte[] actualKey = BitFlip(key);

            rsa.ImportCspBlob(actualKey);
        }

        public byte[] Encrypt(byte[] message)
        {
            return rsa.Encrypt(message, true);
        }

        public byte[] Decrypt(byte[] message)
        {
            return rsa.Decrypt(message, true);
        }

        public byte[] BitFlip(byte[] bytes)
        {
            byte[] result = new byte[bytes.Length];

            for(int i = 0; i < bytes.Length; i++)
            {
                result[i] = (byte)((bytes[i] - 255) * -1);
            }

            return result;
        }
    }
}
