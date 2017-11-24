using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intermediate.Game;
using Intermediate;

namespace Server
{
    class Grid
    {
        private int width;
        private int height;
        private List<GameObject> objects;
        private GameObject[,] grid;

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

        public GameObject this [Vector2I position]
        {
            get
            {
                return grid[position.X, position.Y];
            }
            set
            {
                grid[position.X, position.Y] = value;
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
            this.grid = new GameObject[width, height];
        }

        public GridContainer ToGridContainer()
        {
            List<GameObjectContainer> gameObjectContainers = new List<GameObjectContainer>();

            for (int i = 0; i < objects.Count; i++)
                gameObjectContainers.Add(objects[i].ToGameObjectContainer());

            return new GridContainer(width, height, gameObjectContainers.ToArray());
        }

        private void AddGameObject(GameObject go)
        {
            Vector2I[] positions = GameShapeHelper.GetShape(go.Shape, go.Position);

            for(int i = 0; i < positions.Length; i++)
            {
                if (this[positions[i]] != null)
                    return;
            }

            for(int i = 0; i < positions.Length; i++)
            {
                this[positions[i]] = go;
            }
        }

        private bool ValidatePosition(GameShapes shape, Vector2I position)
        {
            Vector2I[] positions = GameShapeHelper.GetShape(shape, position);

            for (int i = 0; i < positions.Length; i++)
                if (this[positions[i]] != null)
                    return false;

            return true;
        }

        /// <summary>
        /// Returns true if it touches the bottom row.
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="position"></param>
        private bool IsItBottom(GameShapes shape, Vector2I position)
        {
            Vector2I[] positions = GameShapeHelper.GetShape(shape, position);

            for(int i = 0; i < positions.Count(); i++)
                if(positions[i].Y == (grid.GetLength(1) - 1))
                    return true;
            return false;
        }
    }
}
