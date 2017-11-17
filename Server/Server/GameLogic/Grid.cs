using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intermediate.Game;

namespace Server
{
    class Grid
    {
        private int width;
        private int height;
        private List<GameObject> objects;

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

        public List<GameObject> Objects
        {
            get { return objects; }
        }

        public Grid(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.objects = new List<GameObject>();
        }

        public GridContainer ToGridContainer()
        {
            List<GameObjectContainer> gameObjectContainers = new List<GameObjectContainer>();

            for (int i = 0; i < objects.Count; i++)
                gameObjectContainers.Add(objects[i].ToGameObjectContainer());

            return new GridContainer(width, height, gameObjectContainers.ToArray());
        }
    }
}
