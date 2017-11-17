using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intermediate.Game
{
    [Serializable]
    public struct GridContainer
    {
        private int width;
        private int height;
        private List<GameObjectContainer> gameObjects;

        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }

        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }

        public List<GameObjectContainer> GameObjects
        {
            get
            {
                return gameObjects;
            }
            set
            {
                gameObjects = value;
            }
        }

        public GridContainer(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.gameObjects = new List<GameObjectContainer>();
        }

        public GridContainer(int width, int height, params GameObjectContainer[] gameObjects)
            : this(width, height)
        {
            this.gameObjects.AddRange(gameObjects);
        }
    }
}
