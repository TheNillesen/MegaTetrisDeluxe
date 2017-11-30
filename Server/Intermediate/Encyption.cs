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
            //11 is the minimum padding allowed
            int packSize = (rsa.KeySize / 8) - 11;

            int currentIndex = 0;

            byte[] final = new byte[0];

            for(currentIndex = 0; currentIndex < message.Length; currentIndex += packSize)
            {
                int count = packSize + currentIndex > message.Length ? message.Length - currentIndex : packSize;

                byte[] currentPack = new byte[count];

                for (int i = 0; i < currentPack.Length; i++)
                    currentPack[i] = message[currentIndex + i];

                final = final.Concat(rsa.Encrypt(currentPack, false)).ToArray();
            }

            return final;
        }

        public byte[] Decrypt(byte[] message)
        {
            int packSize = rsa.KeySize / 8;
            int currentIndex = 0;

            byte[] final = new byte[0];

            for(currentIndex = 0; currentIndex < message.Length; currentIndex += packSize)
            {
                int count = packSize + currentIndex > message.Length ? message.Length - currentIndex : packSize;

                byte[] currentPack = new byte[count];

                for (int i = 0; i < currentPack.Length; i++)
                    currentPack[i] = message[currentIndex + i];

                final = final.Concat(rsa.Decrypt(currentPack, false)).ToArray();
            }

            return final;
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
