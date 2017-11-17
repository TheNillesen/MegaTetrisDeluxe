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
            int gridWidth = -1;
            int gridHeight = -1;
            long tickCount = -1;

#if DEBUG
            if (args.Length < 4)
                args = new string[] { "port:6666", "width:50", "height:50", "tickCount:100" };
#endif

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Contains("port:"))
                    port = int.Parse(args[i].Split(':').Last());
                else if (args[i].Contains("width:"))
                    gridWidth = int.Parse(args[i].Split(':').Last());
                else if (args[i].Contains("height:"))
                    gridHeight = int.Parse(args[i].Split(':').Last());
                else if (args[i].Contains("tickCount:"))
                    tickCount = long.Parse(args[i].Split(':').Last());
            }

            if (port < 0 || gridWidth < 0 || gridHeight < 0 || tickCount < 0)
                throw new Exception("No port parameter given");

            Console.WriteLine($"Starting server on port {port}, with a grid {gridWidth} wide and {gridHeight} high, one tick is {tickCount} ticks long");

            ConnectionHandler.Init(port);
            GameLogic.GameWorld.Init(gridWidth, gridHeight, tickCount);

            Console.ReadKey();
        }
    }
}
