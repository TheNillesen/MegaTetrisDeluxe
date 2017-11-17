using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Intermediate;
using Andreas.Gade;
using Anders.Vestergaard;
using Nikolaj.Er.Kongen.Men.Han.gider.Ikke.Vise.Det;


namespace Client
{
    class GameClient
    {
        private static GameClient instance = null;
        public static GameClient Instance
        {
            get
            {
                return instance == null ? instance = new GameClient() : instance;
            }
        }

        private Dictionary<string, Func<string, Task>> commandHandlers;
        private TcpClient Client;
        private NetworkStream ServerStream = null;
        private bool jogging = false;

        private GameClient()
        {
            commandHandlers = new Dictionary<string, Func<string, Task>>();
            Client = new TcpClient();                       
        }

        public void Connect(IPAddress ip, int port)
        {
            try
            {
                Client.Connect(ip, port);
            }
            catch (Exception ex)
            {

            }

            if (Client.Connected)
            {
                Client.Client.Send(Encoding.ASCII.GetBytes("Floof"));
                ServerStream = Client.GetStream();
                jogging = true;

                commandHandlers["message"] = HandleMessage;
                commandHandlers["input"] = HandleInput;

                Run();
            }
        }

        private async Task HandleIncomingPackets()
        {
            try
            {
                // Check for new incomding messages
                if (Client.Available > 0)
                {
                    // There must be some incoming data, the first two bytes are the size of the Packet
                    byte[] lengthBuffer = new byte[2];
                    await ServerStream.ReadAsync(lengthBuffer, 0, 2);
                    ushort packetByteSize = BitConverter.ToUInt16(lengthBuffer, 0);
                    // Now read that many bytes from what's left in the stream, it must be the Packet
                    byte[] jsonBuffer = new byte[packetByteSize];
                    await ServerStream.ReadAsync(jsonBuffer, 0, jsonBuffer.Length);
                    // Convert it into a packet datatype
                    NetworkPacket packet = NetworkPacket.Deserialize(jsonBuffer);
                    // Dispatch it
                    try
                    {
                        await commandHandlers[packet.Head](packet.Sender);
                    }
                    catch (KeyNotFoundException)
                    {
                    }
                }
            }
            catch (Exception) { }
        }

        private Task HandleMessage(string message)
        {
            return Task.FromResult(0);
        }

        public async Task HandleInput(string message)
        {
            // Print the prompt and get a response to send
            string responseMsg = "\n\n\n\n\n\0floof";

            // Send the response
            NetworkPacket resp = new NetworkPacket("input", responseMsg);
            await SendPacket(resp);
        }

        public async Task SendPacket(NetworkPacket packet)
        {
            try
            {
                // Convert JSON to buffer and its length to a 16 bit unsigned integer buffer
                byte[] packetBytes = packet.Serialize();
                byte[] lengthBuffer = BitConverter.GetBytes(Convert.ToUInt16(packetBytes.Length));

                // Join the buffers
                byte[] packetBuffer = new byte[lengthBuffer.Length + packetBytes.Length];
                lengthBuffer.CopyTo(packetBuffer, 0);
                packetBytes.CopyTo(packetBuffer, lengthBuffer.Length);

                // Send the packet
                await ServerStream.WriteAsync(packetBuffer, 0, packetBuffer.Length);
            }
            catch (Exception) { }
        }

        public void Run()
        {
            bool wasRunning = jogging;
            // Listen for messages
            List<Task> tasks = new List<Task>();
            while (jogging)
            {
                // Check for new packets
                tasks.Add(HandleIncomingPackets());
                Thread.Sleep(10);
            }
            // Just incase we have anymore packets, give them one second to be processed
            Task.WaitAll(tasks.ToArray(), 1000);
            if (wasRunning)
                Console.WriteLine("Disconnected.");
        }
    }
}
