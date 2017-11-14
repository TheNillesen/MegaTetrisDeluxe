using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    internal static class ConnectionHandler
    {
        private static int port;

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

        private static Dictionary<IPAddress, Socket> connections;

        static ConnectionHandler()
        {
            ConnectionHandler.connections = new Dictionary<IPAddress, Socket>();
        }

        public static void Init(int port)
        {
            ConnectionHandler.port = port;
        }
    }
}
