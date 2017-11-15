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

            new System.Threading.Thread(ServerLoop).Start();
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
                else Thread.Sleep(100);
            }
        }

        private static async Task HandleNewConnection()
        {
            TcpClient newClient = await listner.AcceptTcpClientAsync();
            Console.WriteLine("New connection from {0}.", newClient.Client.RemoteEndPoint);


            clients.Add(newClient);
            
            //await SendToAll(count);
            //await Sendpacket(newClient, new GamePacket("message", msg));
        }

        public static async Task SendPacket(TcpClient client, GamePacket packet)
        {
            try
            {
                // Convert JSON to buffer and its length to a 16 bit unsigned buffer
                byte[] jsonBuffer = Encoding.UTF8.GetBytes(packet.ToJson());
                byte[] lengthBuffer = BitConverter.GetBytes(Convert.ToUInt16(jsonBuffer.Length));

                // Join the buffers
                byte[] msgBuffer = new byte[lengthBuffer.Length + jsonBuffer.Length];
                lengthBuffer.CopyTo(msgBuffer, 0);
                jsonBuffer.CopyTo(msgBuffer, lengthBuffer.Length);

                // Send the packet
                await client.GetStream().WriteAsync(msgBuffer, 0, msgBuffer.Length);
            }
            catch (Exception e)
            {
                // There was and issue when sending
                Console.WriteLine("There was an issue receiving a packet.");
                Console.WriteLine("Reason {0}", e.Message);
            }
        }
        public async static Task<GamePacket> ReceivePacket(TcpClient client)
        {
            GamePacket packet = null;
            try
            {
                // First check there is data available
                if (client.Available == 0)
                    return null;

                NetworkStream msgStream = client.GetStream();

                // There must be some incoming data, the first two bytes are the size of the Packet
                byte[] lengthBuffer = new byte[2];
                await msgStream.ReadAsync(lengthBuffer, 0, 2);
                ushort packetByteSize = BitConverter.ToUInt16(lengthBuffer, 0);

                // Now read that many bytes from what's left in the stream, it must be the Packet
                byte[] jsonBuffer = new byte[packetByteSize];
                await msgStream.ReadAsync(jsonBuffer, 0, jsonBuffer.Length);

                // Convert it into a packet datatype
                string jsonString = Encoding.UTF8.GetString(jsonBuffer);
                packet = GamePacket.FromJson(jsonString);
            }
            catch (Exception e)
            {
                // There was an issue in receiving
                Console.WriteLine("There was an issue sending a packet to {0}", client.Client.RemoteEndPoint);
                Console.WriteLine("Reason: {0}", e.Message);
            }

            return packet;
        }
    }
}
