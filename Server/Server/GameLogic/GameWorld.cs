using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Server.GameLogic
{
    static class GameWorld
    {
        private static Grid grid;
        private static TimeSpan tickTime;
        private static DateTime lastTick;
        private static bool run;

        private static Thread updateThread;
        private static List<GameObject> gameObjects;

        public static List<GameObject> GameObjects
        {
            get { return gameObjects; }
            set { gameObjects = value; }
        }

        public static Grid Grid
        {
            get { return grid; }
            set { grid = value; }
        }

        static GameWorld()
        { run = true; }

        public static void Init(int width, int height, int tickCount)
        {
            grid = new Grid(width, height);
            lastTick = DateTime.Now;
            tickTime = new TimeSpan(0, 0, tickCount);

            updateThread = new Thread(Update);
            updateThread.Start();
        }

        public static void Update()
        {
            while (run)
            {
                if (DateTime.Now - lastTick > tickTime)
                {
                    lastTick = DateTime.Now;
                    OnTick();
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }

        public static void OnTick()
        {
            grid.Objects.ForEach(o => o.Position[0] += new Intermediate.Vector2I(0, 1));

            byte[] temp = new Intermediate.NetworkPacket("Tick", "Server", null).Serialize();

            NewConnectionHandler.SendAll(new Intermediate.NetworkPacket("Tick", "Server", null));
        }
    }
}
