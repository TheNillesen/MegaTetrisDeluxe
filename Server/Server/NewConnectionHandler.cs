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
using Intermediate;

namespace Server
{
    struct Ship
    {
        public TcpClient client;
        public byte[] mess;
    }

    static class NewConnectionHandler
    {
        static TcpListener list;
        static Thread loopster;

        static List<Client> clients;
        static Queue<Ship> bytes;

        static bool requestingGridContainer;
        static Queue<Client> awaitingGrid;

        static Encryption decrypter;

        public static Encryption Decrypter { get { return decrypter; } }

        static NewConnectionHandler()
        {
            clients = new List<Client>();
            bytes = new Queue<Ship>();
            awaitingGrid = new Queue<Client>();
            decrypter = new Encryption();
        }
        public static void Init(int port)
        {
            list = new TcpListener(IPAddress.Any, port);
            list.Start();

            loopster = new Thread(InternalLoop);
            loopster.Start();
        }
        private static void InternalLoop()
        {
            while (true)
            {
                while (list.Pending())
                    AcceptClient();
                Thread.Sleep(10);
            }
        }

        private static void AcceptClient()
        {
            TcpClient client = list.AcceptTcpClient();
            
            Console.WriteLine($"New connection from: {client.Client.RemoteEndPoint.ToString()}");
            Client cl = new Client(client, HandlePackage, Disconnect, Guid.NewGuid());

            NetworkPacket nPacket = new NetworkPacket("ID", "Server", cl.id, decrypter.GetPublicKey());
            SendUnEncrypted(nPacket.Serialize(), cl);

            if (!requestingGridContainer)
            {
                if (clients.Count > 0)
                {
                    requestingGridContainer = true;

                    NetworkPacket networkPacket = new NetworkPacket("Grid_Request", "Server");

                    int i = 0;

                    for (i = 0; i < clients.Count; i++)
                        if (Send(networkPacket.Serialize(), clients[i]))
                        {
                            awaitingGrid.Enqueue(cl);
                            break;
                        }

                    if (i == clients.Count)
                    {
                        NetworkPacket pack = new NetworkPacket("Grid", "Server", GameLogic.GameWorld.Grid.ToGridContainer());
                        cl.packet.Enqueue(pack);

                        requestingGridContainer = false;
                    }
                }
                else
                {
                    NetworkPacket networkPacket = new NetworkPacket("Grid", "Server", GameLogic.GameWorld.Grid.ToGridContainer());
                    cl.packet.Enqueue(networkPacket);
                }
            }

            clients.Add(cl);
        }

        private static void Disconnect(Client client)
        {
            Console.WriteLine($"Disconnecting {client.id.ToString()} from {client.client.Client.RemoteEndPoint.ToString()}");

            clients.Remove(client);
            client.Run = false;
        }

        private static bool SendUnEncrypted(byte[] message, Client client)
        {
            try
            {
                lock (client.client)
                {
                    byte[] length = BitConverter.GetBytes(message.Length);

                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(length);

                    byte[] pack = new byte[length.Length + message.Length];

                    length.CopyTo(pack, 0);
                    message.CopyTo(pack, length.Length);

                    client.client.GetStream().Write(pack, 0, pack.Length);
                }

                return true;
            }

            catch (Exception ex)
            {
                if (!client.client.Connected)
                {
                    Console.WriteLine($"Closing connection to {client.client.Client.RemoteEndPoint.ToString()}, Reason: {ex.Message}");

                    clients.Remove(client);
                    client.Run = false;
                }

                return false;
            }
        }

        private static bool Send(byte[] mess, Client client)
        {
            try
            {
                lock (client.client)
                {
                    byte[] message = client.encrypter.Encrypt(mess);

                    byte[] length = BitConverter.GetBytes(message.Length);

                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(length);

                    byte[] pack = new byte[length.Length + message.Length];

                    length.CopyTo(pack, 0);
                    message.CopyTo(pack, length.Length);

                    client.client.GetStream().Write(pack, 0, pack.Length);
                }

                return true;
            }

            catch (Exception ex)
            {
                if (!client.client.Connected)
                {
                    Console.WriteLine($"Closing connection to {client.client.Client.RemoteEndPoint.ToString()}, Reason: {ex.Message}");

                    clients.Remove(client);
                    client.Run = false;
                }

                return false;
            }
        }

        public static void SendAll(NetworkPacket nPacket, TcpClient ignore = null)
        {
            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].packet.Enqueue(nPacket);
            }
        }
        private static void HandlePackage(NetworkPacket nPacket)
        {
            if (nPacket.Head != "Action")
                Console.WriteLine($"Handling {nPacket.Head} from {nPacket.Sender}");

            switch (nPacket.Head)
            {
                case "Action":
                    SendAll(nPacket);
                    break;
                case "Spawn":
                    SendAll(nPacket);
                    break;
                case "Grid":
                    nPacket.Sender = "Server";

                    while (awaitingGrid.Count != 0)
                    {
                        Client cl = awaitingGrid.Dequeue();

                        cl.packet.Enqueue(nPacket);
                    }

                    requestingGridContainer = false;
                    break;
                case "Key":
                    Client c = clients.Find(o => o.id.ToString() == nPacket.Sender);

                    c.encrypter = new Encryption((byte[])nPacket.Data[0]);
                    break;
                default:
                    return;
            }
        }
    }
}
