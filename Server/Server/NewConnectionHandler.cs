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
        public  TcpClient client;
        public  byte[] mess;
    }

    static class NewConnectionHandler
    {
        static TcpListener list;
        static Thread loopster;

        static List<TcpClient> clients;
        static Queue<Ship> bytes;

        static NewConnectionHandler()
        {
            clients = new List<TcpClient>();
            bytes = new Queue<Ship>();
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

                    s.client.GetStream().Write(s.mess, 0, s.mess.Length);
                }
            }
        }

        private static void AcceptClient()
        {
            TcpClient client = list.AcceptTcpClient();

            clients.Add(client);

            NetworkPacket nPacket = new NetworkPacket("ID","Server",Guid.NewGuid(),GameLogic.GameWorld.Grid.ToGridContainer());
            Send(nPacket.Serialize(), client);
            Console.WriteLine($"New connection from: {client.Client.RemoteEndPoint.ToString()}");
        }

        private static void Receive(TcpClient client)
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
                NetworkPacket nPacket = NetworkPacket.Deserialize(message);
                HandlePakage(nPacket);
            }
            catch (Exception ex)
            { Console.WriteLine(ex.Message); }
        }

        private static string ToString(byte[] mess)
        {
            BinaryFormatter bf = new BinaryFormatter();
            string res;

            using (MemoryStream memS = new MemoryStream(mess))
            {
                res = bf.Deserialize(memS) as string;
            }

            return res;
        }

        private static void Send(byte[] message, TcpClient client)
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
            catch (Exception ex)
            { Console.WriteLine(ex.Message); }
        }
        public static void SendAll(NetworkPacket nPacket, TcpClient ignore = null)
        {
            if(ignore != null)
            {
                TcpClient[] tcpArray = clients.FindAll(o => o != ignore).ToArray();
                for (int i = 0; i < tcpArray.Length; i++)
                {
                    Send(nPacket.Serialize(), tcpArray[i]);
                }
                
            }
            else
            {
                for (int i = 0; i < clients.Count; i++)
                {
                    Send(nPacket.Serialize(), clients[i]);
                }
            }
        }
        private static void HandlePakage(NetworkPacket nPacket)
        {
            switch (nPacket.Head)
            {
                case "Move":
                    GameLogic.GameWorld.Grid.Objects.Find(o => o.Guid == nPacket.Sender).Position[0] += (Vector2I)nPacket.Data[0];
                    SendAll(nPacket);
                    break;
                case "Spawn":
                    GameObject go = new GameObject();
                    go.Guid = nPacket.Sender;
                    go.Position = GameShapeHelper.GetShape((GameShapes)nPacket.Data[1], (Vector2I)nPacket.Data[0]);
                    go.Shape = (GameShapes)nPacket.Data[1];
                    GameLogic.GameWorld.Grid.AddGameObject(go);
                    SendAll(nPacket);
                    break;
                default:
                    return;

            }
        }
    }
}
