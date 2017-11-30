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

        public Guid Guid { get { return ID; } }

        private IPEndPoint connectionInfo;

        public Client()
        {
            messages = new Queue<NetworkPacket>();
        }

        public void Connect(IPAddress address, int port)
        {
            connectionInfo = new IPEndPoint(address, port);

            client = new TcpClient();

            client.Connect(connectionInfo);

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
                {
                    NetworkPacket np = messages.Dequeue();
                    if (!SendInternal(np.Serialize()))
                    {
                        messages.Enqueue(np);
                    }
                }
                Thread.Sleep(10);
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

        private bool SendInternal(byte[] mess)
        {
            try
            {
                byte[] length = new byte[sizeof(int)];

                length = BitConverter.GetBytes(mess.Length);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(length);

                byte[] packet = new byte[length.Length + mess.Length];

                length.CopyTo(packet, 0);
                mess.CopyTo(packet, length.Length);

                client.GetStream().Write(packet, 0, packet.Length);

                return true;
            }
            catch (Exception ex)
            {
                if (!client.Connected)
                {
                    client = new TcpClient();
                    client.Connect(connectionInfo);
                }
                return false;
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

                int ulength = BitConverter.ToInt32(length, 0);

                byte[] mess = new byte[ulength];
                recv = 0;
                
                while(recv < ulength)
                    recv += client.GetStream().Read(mess, recv, mess.Length - recv);

                NetworkPacket nPacket = NetworkPacket.Deserialize(mess);

                HandlePackage(nPacket);
            }
            catch (Exception ex)
            { }
        }
        private void HandlePackage(NetworkPacket nPacket)
        {
            if (nPacket.Sender == ID.ToString())
                return;

            switch (nPacket.Head)
            {
                case "Action":
                    Guid senderID = new Guid(nPacket.Sender);

                    Transform transform = Gameworld.Instance.gameObjects.Find(o => o.GetComponent<NetworkController>(n => n.ID == senderID) != null)?.Transform;

                    if (transform == null)
                    { 
                        ///Do Stuff
                    }
                    else
                    {
                        switch((Intermediate.Action)nPacket.Data[0])
                        {
                            case Intermediate.Action.Down:
                                transform.MoveDown();
                                break;
                            case Intermediate.Action.Left:
                                transform.MoveLeft();
                                break;
                            case Intermediate.Action.Rigth:
                                transform.MoveRight();
                                break;
                            case Intermediate.Action.RotateLeft:
                                transform.RotateLeft();
                                break;
                            case Intermediate.Action.RotateRight:
                                transform.RotateRight();
                                break;
                            case Intermediate.Action.Instant:
                                transform.PlaceBlockNow();
                                break;
                        }
                    }
                    break;
                case "ID":
                    if (nPacket.Sender == "Server")
                    {
                        ID = (Guid)nPacket.Data[0];
                    }
                    break;
                case "Spawn":
                    GameObject go = new GameObject();
                    go.AddComponent(new Spriterendere(go, "GreyToneBlock", 1));
                    go.AddComponent(new Transform(go, (Vector2I)nPacket.Data[0], (GameShapes)nPacket.Data[1]));
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
                case "Grid_Request":
                    if (nPacket.Sender == "Server")
                    {
                        NetworkPacket pack = new NetworkPacket("Grid", ID.ToString());

                        pack.Data.Add(Gameworld.Instance.gameMap.ToContainer());

                        SendInternal(pack.Serialize());
                    }
                    break;
                case "Grid":
                    if (nPacket.Sender == "Server")
                    {
                        Gameworld.Instance.gameMap.FromContainer((GridContainer)nPacket.Data[0]);
                    }
                    break;
                default:
                    return;
                    
            }
        }
    }
}

