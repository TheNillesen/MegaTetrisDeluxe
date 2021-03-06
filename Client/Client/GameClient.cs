﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Intermediate;
using Microsoft.Xna.Framework;
using Andreas.Gade;
using Anders.Vestergaard;
using Microsoft.Xna.Framework.Media;

namespace Client
{
    class GameClient
    {

        private Queue<NetworkPacket> networkPakages; 
        private Dictionary<string, Func<NetworkPacket, Task>> commandHandlers;
        private TcpClient Client;
        private NetworkStream ServerStream = null;
        private bool jogging = false;
        private Guid id;

        public Guid Id
        {
            get { return id; }
            set { id = value; }
        }

        public GameClient()
        {
            networkPakages = new Queue<NetworkPacket>();
            commandHandlers = new Dictionary<string, Func<NetworkPacket, Task>>();
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
                commandHandlers["Tick"] = HandleTick;
                commandHandlers["Move"] = HandleMove;
                commandHandlers["Spawn"] = HandleSpawn;
                commandHandlers["ID"] = HandleID;

                Run();
            }
        }

        private async Task HandleID(NetworkPacket p)
        {
            if (p.Sender == "Server")
            {
                id = (Guid)p.Data[0];
                Gameworld.Instance.gameMap.FromContainer((Intermediate.Game.GridContainer)p.Data[1]);
            }
        }

        private async Task HandleTick(NetworkPacket s)
        {
            if (s.Sender == "Server")
            {
                Gameworld.Instance.OnTick();
            }
        }

        private async Task HandleMove(NetworkPacket packet)
        {
            Transform tf = Gameworld.Instance.GetGameobject(o => o.GetComponent<NetworkController>(n => n.ID.ToString() == packet.Sender) != null)?.Transform;

            if (tf != null)
                tf.Position[0] += ((Vector2I)packet.Data[0]).ToVector2();
        }

        private async Task HandleSpawn(NetworkPacket packet)
        {
            GameObject go = new GameObject();

            go.AddComponent(new Transform(go, (Vector2I)packet.Data[0], (GameShapes)packet.Data[1]));
            go.AddComponent(new NetworkController(go, new Guid(packet.Sender)));
            go.AddComponent(new Spriterendere(go, "GreyToneBlock", 1f));
            go.LoadContent(Gameworld.Instance.Content);
            Gameworld.Instance.AddGameObject(go);
        }

        private async Task HandleIncomingPackets()
        {
            try
            {
                // Check for new incomding messages
                if (Client.Available > sizeof(ulong))
                {
                    // There must be some incoming data, the first two bytes are the size of the Packet
                    byte[] lengthBuffer = new byte[sizeof(ulong)];

                    if (BitConverter.IsLittleEndian)
                    {
                        lengthBuffer.Reverse();
                    }

                    await ServerStream.ReadAsync(lengthBuffer, 0, sizeof(ulong));
                    ulong packetByteSize = BitConverter.ToUInt64(lengthBuffer, 0);
                    // Now read that many bytes from what's left in the stream, it must be the Packet
                    byte[] packetBuffer = new byte[packetByteSize];
                    await ServerStream.ReadAsync(packetBuffer, 0, packetBuffer.Length);
                    // Convert it into a packet datatype
                    NetworkPacket packet = NetworkPacket.Deserialize(packetBuffer);

                    // Dispatch it
                    try
                    {
                        await commandHandlers[packet.Head](packet);
                    }
                    catch (KeyNotFoundException)
                    {
                    }
                }
            }
            catch (Exception ex)
            { }
        }

        private Task HandleMessage(NetworkPacket message)
        {
            return Task.FromResult(0);
        }

        public async Task HandleInput(NetworkPacket message)
        {
            // Print the prompt and get a response to send
            string responseMsg = "\n\n\n\n\n\0floof";

            // Send the response
            NetworkPacket resp = new NetworkPacket("input", responseMsg);
            SendPacket(resp);
        }
        public void SendPacket(NetworkPacket networkPacket)
        {
            networkPakages.Enqueue(networkPacket);
        }
        private void SendPacketInternal(NetworkPacket packet)
        {
            if (packet.Sender == null)
                packet.Sender = id.ToString();

            try
            {
                // Convert JSON to buffer and its length to a 16 bit unsigned integer buffer
                byte[] packetBytes = packet.Serialize();
                byte[] lengthBuffer = BitConverter.GetBytes(Convert.ToUInt64(packetBytes.Length));

                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(lengthBuffer);
                }

                // Join the buffers
                byte[] packetBuffer = new byte[lengthBuffer.Length + packetBytes.Length];
                lengthBuffer.CopyTo(packetBuffer, 0);
                packetBytes.CopyTo(packetBuffer, lengthBuffer.Length);

                // Send the packet
                ServerStream.Write(packetBuffer, 0, packetBuffer.Length);
            }
            catch (Exception ex) { }
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
                while (networkPakages.Count > 0)
                {
                    SendPacketInternal(networkPakages.Dequeue());
                }
            }
            // Just incase we have anymore packets, give them one second to be processed
            Task.WaitAll(tasks.ToArray(), 1000);
            if (wasRunning)
                Console.WriteLine("Disconnected.");
        }
    }
}
