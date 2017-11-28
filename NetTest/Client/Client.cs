using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Client
{
    class Client
    {
        TcpClient client;
        Thread loopster;
        Queue<byte[]> messages;

        public Client()
        {
            messages = new Queue<byte[]>();
        }

        public void Connect(IPAddress address, int port)
        {
            client = new TcpClient();

            client.Connect(address, port);

            loopster = new Thread(Loopster);
            loopster.Start();
        }

        private void Loopster()
        {
            while(true)
            {
                while (client.Available > sizeof(ulong))
                    Receive();
                while (messages.Count > 0)
                    SendInternal(messages.Dequeue());
            }
        }

        public void Send(byte[] message)
        {
            messages.Enqueue(message);
        }

        private string ToString(byte[] mess)
        {
            BinaryFormatter bf = new BinaryFormatter();
            string res;

            using (MemoryStream memS = new MemoryStream(mess))
            {
                res = bf.Deserialize(memS) as string;
            }

            return res;
        }

        private void SendInternal(byte[] mess)
        {
            try
            {
                byte[] length = new byte[sizeof(ulong)];

                length = BitConverter.GetBytes((ulong)mess.Length);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(length);

                byte[] packet = new byte[length.Length + mess.Length];

                length.CopyTo(packet, 0);
                mess.CopyTo(packet, length.Length);

                client.GetStream().Write(packet, 0, packet.Length);
            }
            catch(Exception ex)
            { }
        }

        private void Receive()
        {
            try
            {
                byte[] length = new byte[sizeof(ulong)];

                client.GetStream().Read(length, 0, sizeof(ulong));

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(length);

                ulong ulength = BitConverter.ToUInt64(length, 0);

                byte[] mess = new byte[ulength];

                client.GetStream().Read(mess, 0, mess.Length);
            }
            catch(Exception ex) { }
        }
    }
}
