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
using Intermediate;
using Intermediate.Game;

namespace Client
{
    class Client
    {
        Guid ID;
        TcpClient client;
        Thread loopster;
        Queue<NetworkPacket> messages;

        public Client()
        {
            messages = new Queue<NetworkPacket>();
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
            while (true)
            {
                while (client.Available > sizeof(ulong))
                    Receive();
                while (messages.Count > 0)
                    SendInternal(messages.Dequeue().Serialize());
            }
        }

        public void Send(NetworkPacket message)
        {
            if(message.Sender == null)
            {
                message.Sender = ID.ToString();
            } 
            messages.Enqueue(message);
        }

        private NetworkPacket ToString(byte[] mess)
        {
            return NetworkPacket.Deserialize(mess);  
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
            catch (Exception ex)
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
                NetworkPacket nPacket = NetworkPacket.Deserialize(mess);
            }
            catch (Exception ex) { }
        }
        private void HandlePakage(NetworkPacket nPacket)
        {
            if (nPacket.Sender == ID.ToString())
                return;
            switch (nPacket.Head)
            {
                case "Move":
                    Guid senderID = new Guid(nPacket.Sender);
                    Gameworld.Instance.gameObjects.Find(o => o.GetComponent<NetworkController>(n => n.ID == senderID) != null).Transform.Translate(((Vector2I)(nPacket.Data[0])).ToVector2());
                    break;
                case "ID":
                    if (nPacket.Sender == "Server")
                    {
                        ID = (Guid)nPacket.Data[0];
                        Gameworld.Instance.gameMap.FromContainer((GridContainer)nPacket.Data[1]);
                    }
                    break;
                case "Spawn":
                    GameObject go = new GameObject();
                    go.AddComponent(new Transform(go, (Vector2I)nPacket.Data[0], (GameShapes)nPacket.Data[1]));
                    go.AddComponent(new Spriterendere(go, "GreyToneBlock", 1));
                    go.AddComponent(new NetworkController(go, new Guid(nPacket.Sender)));
                    Gameworld.Instance.AddGameObject(go);
                    go.LoadContent(Gameworld.Instance.Content);
                    break;
                case "Tick":
                    if(nPacket.Sender == "Server")
                    {
                        Gameworld.Instance.OnTick();
                    }
                    
                    break;

                default:
                    return;
                    
            }
        }
    }
}

