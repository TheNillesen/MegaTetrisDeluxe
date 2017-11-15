using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(params string[] args)
        {
            int port = -1;

#if DEBUG
            args = new string[] { "port:6666" };
#endif
            if (args.Length <= 0)
                throw new Exception("No port parameter given");

            else
                for (int i = 0; i < args.Length; i++)
                    if (args[i].Contains("port:"))
                        port = int.Parse(args[i].Split(':').Last());

            if (port < 0)
                throw new Exception("No port parameter given");

            ConnectionHandler.Init(port);

            Console.ReadKey();
        }
    }
}
