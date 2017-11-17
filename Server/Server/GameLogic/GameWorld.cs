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

        static GameWorld()
        { }

        public static void Init(int width, int height, long tickCount)
        {
            grid = new Grid(width, height);
            lastTick = DateTime.Now;
            tickTime = new TimeSpan(tickCount);
        }

        public static void Update()
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

        public static void OnTick()
        {
            
        }
    }
}
