using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server.Net
{
    static class ClientComm
    {
        private struct Client
        {
            public TcpClient client { get; private set; }
            public Guid id { get; private set; }

            public Client(TcpClient client)
            {
                this.client = client;
                this.id = Guid.NewGuid();
            }
        }

        private static Dictionary<Guid, Client> clients;
        private static TcpListener listener;
        private static Thread listenerThread;

        private static List<Task> tasks;

        private static bool run;

        static ClientComm()
        {
            clients = new Dictionary<Guid, Client>();
            tasks = new List<Task>();
        }

        public static void Init(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();

            run = true;

            listenerThread = new Thread(ListenerLoop);
            listenerThread.Start();
        }

        private static void ListenerLoop()
        {
            while (run)
            {
                if (listener.Pending())
                {
                    tasks.Add(NewConnection());
                }
            }
        }

        private static async Task NewConnection()
        {
            Client c = new Client(await listener.AcceptTcpClientAsync());

            Console.WriteLine($"Received Connection from {c.client.Client.RemoteEndPoint.ToString()}");

            clients.Add(c.id, c);

            tasks.RemoveAll(o => o.Id == Task.CurrentId);
        }

        public static void SendPacket(Intermediate.NetworkPacket packet, Guid id)
        {
            tasks.Add(delegate() { SendPacket(packet, id); });
        }

        private static async Task SendPacket(Intermediate.NetworkPacket packet, Guid id, bool everyoneButID = false)
        {
            Client client = clients[id];

            byte[] bytes = packet.Serialize();
            byte[] length = BitConverter.GetBytes((ulong)bytes.Length);

            if (BitConverter.IsLittleEndian)
            {
                length.Reverse();
            }

            byte[] finalPacket = new byte[bytes.Length + length.Length];

            length.CopyTo(finalPacket, 0);
            bytes.CopyTo(finalPacket, length.Length);

            await client.client.GetStream().WriteAsync(finalPacket, 0, finalPacket.Length);
        }

        private static async Task SendPacketAll(Intermediate.NetworkPacket packet, Guid toIgnore = default(Guid))
        {
            foreach (Guid id in clients.Keys)
                if (id != toIgnore)
                    await SendPacket(packet, id);
        }
    }
}
