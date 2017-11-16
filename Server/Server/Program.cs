using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intermediate;

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

            for (int i = 0; i < args.Length; i++)
                if (args[i].Contains("port:"))
                    port = int.Parse(args[i].Split(':').Last());

            if (port < 0)
                throw new Exception("No port parameter given");

            Vector2I vec = new Vector2I(2, 5);

            byte[] temp = vec.Serialize();

            Vector2I vecTemp = Vector2I.Deserialize(temp);

            Console.WriteLine(Encoding.UTF8.GetString(temp));
            Console.WriteLine($"{vecTemp.X}:{vecTemp.Y}");

            ConnectionHandler.Init(port);

            Console.ReadKey();
        }
    }
}
