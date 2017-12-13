using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Server
{
    struct Ship
    {
        public TcpClient client;
        public byte[] mess;
    }

    class ConHan
    {
        TcpListener list;
        Thread loopster;

        List<TcpClient> clients;
        Queue<Ship> bytes;

        public ConHan(int port)
        {
            list = new TcpListener(IPAddress.Any, port);
            clients = new List<TcpClient>();
            bytes = new Queue<Ship>();

            list.Start();

            loopster = new Thread(InternalLoop);
            loopster.Start();
        }

        private void InternalLoop()
        {
            while (true)
            {
                while (list.Pending())
                    AcceptClient();
                for (int i = 0; i < clients.Count; i++)
                    if (clients[i].Available > sizeof(ulong))
                        Receive(clients[i]);
                for (int i = 0; i < bytes.Count; i++)
                {
                    Ship s = bytes.Dequeue();

                    s.client.GetStream().Write(s.mess, 0, s.mess.Length);
                }
            }
        }

        private void AcceptClient()
        {
            TcpClient client = list.AcceptTcpClient();

            clients.Add(client);
        }

        private void Receive(TcpClient client)
        {
            try
            {
                byte[] length = new byte[sizeof(ulong)];

                client.GetStream().Read(length, 0, sizeof(ulong));

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(length);

                ulong lengths = BitConverter.ToUInt64(length, 0);
                
                byte[] message = new byte[lengths];
                client.GetStream().Read(message, 0, (int)lengths);

                Console.WriteLine(ToString(message));

                IEnumerable<TcpClient> clients = this.clients.FindAll(o => o != client);

                foreach(TcpClient cl in clients)
                    Send(message, cl);
            }
            catch(Exception ex)
            { }
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

        private void Send(byte[] message, TcpClient client)
        {
            try
            {
                byte[] length = BitConverter.GetBytes((ulong)message.Length);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(length);

                byte[] pack = new byte[length.Length + message.Length];

                length.CopyTo(pack, 0);
                message.CopyTo(pack, length.Length);

                client.GetStream().Write(pack, 0, pack.Length);
            }
            catch { }
        }
    }
}
