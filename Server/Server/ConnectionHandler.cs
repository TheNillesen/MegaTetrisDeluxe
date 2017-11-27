using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Intermediate;

namespace Server
{
    
    internal static class ConnectionHandler
    {
        private static TcpListener listner;
        private static int port;
        private static bool serverRunning;
            
        public static int Port
        {
            get
            {
                return port;
            }
            set
            {
                port = value;
            }
        }

        private static List<TcpClient> clients;

        static ConnectionHandler()
        {
            ConnectionHandler.clients = new List<TcpClient>();
        }

        public static void Init(int port)
        {
            ConnectionHandler.port = port;
            ConnectionHandler.listner = new TcpListener(IPAddress.Any, port);

         
            listner.Start();
            serverRunning = true;

            new Thread(ServerLoop).Start();
        }

        public static void ServerLoop()
        {
            List<Task> newConnectionTasks = new List<Task>();

            while (serverRunning)
            {
                if (listner.Pending())
                {
                    newConnectionTasks.Add(HandleNewConnection());            
                }
                for (int i = 0; i < clients.Count; i++)
                {
                    ReceivePacket(clients[i]);
                }
            }
        }

        private static async Task HandleNewConnection()
        {
            TcpClient newClient = await listner.AcceptTcpClientAsync();
            Console.WriteLine("New connection from {0}.", newClient.Client.RemoteEndPoint);
            
            clients.Add(newClient);

            await SendPacket(newClient, new NetworkPacket("ID", "Server", Guid.NewGuid(), GameLogic.GameWorld.Grid.ToGridContainer()));

            //await SendToAll(count);
            //await Sendpacket(newClient, new GamePacket("message", msg));
        }

        public static async Task SendPacket(TcpClient client, NetworkPacket packet)
        {
            try
            {
                // Convert JSON to buffer and its length to a 16 bit unsigned buffer
                byte[] packetBuffer = packet.Serialize();
                byte[] lengthBuffer = BitConverter.GetBytes(Convert.ToUInt64(packetBuffer.Length));

                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(lengthBuffer);
                }

                // Join the buffers
                byte[] msgBuffer = new byte[lengthBuffer.Length + packetBuffer.Length];
                lengthBuffer.CopyTo(msgBuffer, 0);
                packetBuffer.CopyTo(msgBuffer, lengthBuffer.Length);

                // Send the packet
                await client.GetStream().WriteAsync(msgBuffer, 0, msgBuffer.Length);
            }
            catch (Exception e)
            {
                if (!client.Connected)
                    clients.Remove(client);

                // There was and issue when sending
                Console.WriteLine("There was an issue receiving a packet.");
                Console.WriteLine("Reason {0}", e.Message);
            }
        }

        public static async Task SendPacketAll(NetworkPacket packet)
        {
            for (int i = 0; i < clients.Count; i++)
                await SendPacket(clients[i], packet);
        }

        public async static Task<NetworkPacket> ReceivePacket(TcpClient client)
        {
            NetworkPacket packet = null;
            try
            {
                // First check there is data available
                if (client.Available == 0)
                    return null;

                NetworkStream msgStream = client.GetStream();

                // There must be some incoming data, the first two bytes are the size of the Packet
                byte[] lengthBuffer = new byte[4];
                int recv = 0;

                while (recv < lengthBuffer.Length)
                {
                    recv += await msgStream.ReadAsync(lengthBuffer, recv, lengthBuffer.Length - recv);
                }                

                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(lengthBuffer);
                }

                int packetByteSize = BitConverter.ToInt32(lengthBuffer, 0);

                // Now read that many bytes from what's left in the stream, it must be the Packet
                byte[] packetBuffer = new byte[packetByteSize];
                await msgStream.ReadAsync(packetBuffer, 0, packetBuffer.Length);

                // Convert it into a packet datatype
                packet = NetworkPacket.Deserialize(packetBuffer);

                switch(packet.Head)
                {
                    case "Spawn":
                        await Spawn(packet);
                        break;
                    case "Move":
                        await Move(packet);
                        break;
                    case "Rotate":
                        await Rotate(packet);
                        break;
                }
            }
            catch (Exception e)
            {
                // There was an issue in receiving
                Console.WriteLine("There was an issue sending a packet to {0}", client.Client.RemoteEndPoint);
                Console.WriteLine("Reason: {0}", e.Message);
            }

            return packet;
        }

        private static GameObject GetCorrospondingGameObject(NetworkPacket packet)
        {
            return GameLogic.GameWorld.GameObjects.Find(o => o.Guid == packet.Sender);
        }

        private static async Task Move (NetworkPacket packet)
        {
            SendPacketAll(packet);

            GameObject go = GetCorrospondingGameObject(packet);

            go.Position[0] += (Vector2I)packet.Data[0];
        }

        private static async Task Rotate(NetworkPacket packet)
        {
            SendPacketAll(packet);

            GameObject go = GetCorrospondingGameObject(packet);

            
        }
        private static async Task Spawn(NetworkPacket packet)
        {
            SendPacketAll(packet);
            GameObject sGo = new GameObject();
            sGo.Guid = packet.Sender;
            sGo.Position = GameShapeHelper.GetShape((GameShapes)packet.Data[1], (Vector2I)packet.Data[0]);
            GameLogic.GameWorld.Grid.AddGameObject(sGo);
        }
    }
}
