using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    class Client
    {
        public Queue<Intermediate.NetworkPacket> packet;
        public TcpClient client;
        public Thread thread;
        public Guid id;
        public Action<Intermediate.NetworkPacket> handlePacket;
        public Action<Client> disconnect;

        public Intermediate.Encryption encrypter;

        public bool Run;

        public Client(TcpClient client, Action<Intermediate.NetworkPacket> packetHandler, Action<Client> disconnect, Guid id)
        {
            packet = new Queue<Intermediate.NetworkPacket>();

            this.client = client;
            this.handlePacket = packetHandler;
            this.disconnect = disconnect;
            this.id = id;
            this.Run = true;

            Console.WriteLine($"Assigning {id} to {client.Client.RemoteEndPoint.ToString()}");

            thread = new Thread(ClientLoop);
            thread.Start();
        }

        private void ClientLoop()
        {
            while (Run)
            {
                while (client.Available > 0)
                    Receive();
                while (encrypter != null && packet.Count > 0)
                {
                    Intermediate.NetworkPacket pack = packet.Dequeue();

                    if (pack.Sender != id.ToString())
                        Send(pack.Serialize());
                }
                Thread.Sleep(10);
            }
        }

        private void Receive()
        {
            try
            {
                byte[] length = new byte[sizeof(int)];
                int recv = 0;

                while (recv < sizeof(int))
                    recv += client.GetStream().Read(length, recv, sizeof(int) - recv);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(length);

                int lengths = BitConverter.ToInt32(length, 0);

                byte[] message = new byte[lengths];
                recv = 0;

                while (recv < lengths)
                    recv += client.GetStream().Read(message, recv, lengths - recv);

                message = NewConnectionHandler.Decrypter.Decrypt(message);

                Intermediate.NetworkPacket nPacket = Intermediate.NetworkPacket.Deserialize(message);
                handlePacket(nPacket);
            }
            catch (Exception ex)
            { Console.WriteLine(ex.Message); }
        }

        private bool Send(byte[] message)
        {
            try
            {
                lock (client)
                {
                    if (encrypter != null)
                        message = encrypter.Encrypt(message);

                    byte[] length = BitConverter.GetBytes(message.Length);

                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(length);

                    byte[] pack = new byte[length.Length + message.Length];

                    length.CopyTo(pack, 0);
                    message.CopyTo(pack, length.Length);

                    client.GetStream().Write(pack, 0, pack.Length);
                }

                return true;
            }

            catch (Exception ex)
            {
                if (!client.Connected)
                {
                    disconnect(this);
                }

                return false;
            }
        }
    }
}
