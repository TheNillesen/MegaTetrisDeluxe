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

        static List<TcpClient> clients;
        static Queue<Ship> bytes;

        static bool requestingGridContainer;
        static Queue<TcpClient> awaitingGrid;

        static NewConnectionHandler()
        {
            clients = new List<TcpClient>();
            bytes = new Queue<Ship>();
            awaitingGrid = new Queue<TcpClient>();
        }
        public static void Init(int port)
        {
            list = new TcpListener(IPAddress.Any, port);
            list.Start();

            loopster = new Thread(InternalLoop);
            loopster.Start();
        }
        private  static void InternalLoop()
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

                    Send(s.mess, s.client);
                }
            }
        }

        private static void AcceptClient()
        {
            TcpClient client = list.AcceptTcpClient();

            NetworkPacket nPacket = new NetworkPacket("ID", "Server", Guid.NewGuid());
            Send(nPacket.Serialize(), client);
            Console.WriteLine($"New connection from: {client.Client.RemoteEndPoint.ToString()}");

            if (!requestingGridContainer)
            {
                if (clients.Count > 0)
                {
                    requestingGridContainer = true;

                    NetworkPacket networkPacket = new NetworkPacket("Grid_Request", "Server");

                    int i = 0;

                    for (i = 0; i < clients.Count; i++)
                        if (Send(networkPacket.Serialize(), clients[i]))
                            break;

                    if (i == clients.Count)
                    {
                        NetworkPacket pack = new NetworkPacket("Grid", "Server", GameLogic.GameWorld.Grid.ToGridContainer());
                        Send(pack.Serialize(), client);

                        requestingGridContainer = false;
                    }
                }
                else
                {
                    NetworkPacket networkPacket = new NetworkPacket("Grid", "Server", GameLogic.GameWorld.Grid.ToGridContainer());
                    Send(networkPacket.Serialize(), client);
                }
            }

            clients.Add(client);
            awaitingGrid.Enqueue(client);
        }

        private static void Receive(TcpClient client)
        {
            try
            {
                byte[] length = new byte[sizeof(int)];
                int recv = 0;

                while(recv < sizeof(int))
                    recv += client.GetStream().Read(length, recv, sizeof(int) - recv);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(length);

                int lengths = BitConverter.ToInt32(length, 0);

                byte[] message = new byte[lengths];
                recv = 0;

                while (recv < lengths)
                    recv += client.GetStream().Read(message, recv, lengths - recv);

                NetworkPacket nPacket = NetworkPacket.Deserialize(message);
                HandlePackage(nPacket);
            }
            catch (Exception ex)
            { Console.WriteLine(ex.Message); }
        }

        private static bool Send(byte[] message, TcpClient client)
        {
            try
            {
                byte[] length = BitConverter.GetBytes(message.Length);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(length);

                byte[] pack = new byte[length.Length + message.Length];

                length.CopyTo(pack, 0);
                message.CopyTo(pack, length.Length);

                client.GetStream().Write(pack, 0, pack.Length);

                return true;
            }

            catch (Exception ex)
            {
                if (!client.Connected)
                {
                    Console.WriteLine($"Closing connection to {client.Client.RemoteEndPoint.ToString()}, Reason: {ex.Message}");

                    clients.Remove(client);
                }

                return false;
            }
        }

        public static void SendAll(NetworkPacket nPacket, TcpClient ignore = null)
        {
            if(ignore != null)
            {
                TcpClient[] tcpArray = clients.FindAll(o => o != ignore).ToArray();
                for (int i = 0; i < tcpArray.Length; i++)
                {
                    Ship s = new Ship();

                    s.client = tcpArray[i];
                    s.mess = nPacket.Serialize();

                    bytes.Enqueue(s);
                }
            }
            else
            {
                for (int i = 0; i < clients.Count; i++)
                {
                    Ship s = new Ship();

                    s.client = clients[i];
                    s.mess = nPacket.Serialize();

                    bytes.Enqueue(s);
                }
            }
        }
        private static void HandlePackage(NetworkPacket nPacket)
        {
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

                    while(awaitingGrid.Count != 0)
                    {
                        Send(nPacket.Serialize(), awaitingGrid.Dequeue());
                    }

                    requestingGridContainer = false;
                    break;
                default:
                    return;
            }
        }
    }
}
