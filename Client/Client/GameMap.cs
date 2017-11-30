using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Intermediate;

namespace Client
{
    class GameMap
    {
        public Vector2 offset;     //The game areas position in the window.
        public int gameAreaWidth;  //The area width in which the game it self takes place.
        public int gameAreaHeight; //The area height in which the game it self takes place.
        public GameObject[,] map; //The map grid.
        public float cellWidth;   //The width of a cell.
        public float cellHeight;  //The height of a cell.

        private List<GameObject> visual;

        /// <summary>
        /// Auto generates the cells dimensions.
        /// </summary>
        /// <param name="numberOfCellsWidth"></param>
        /// <param name="numberOfCellsHeight"></param>
        /// <param name="gameAreaWidth"></param>
        /// <param name="gameAreaHeight"></param>
        /// <param name="offset"></param>
        public GameMap(int numberOfCellsWidth, int numberOfCellsHeight, int gameAreaWidth, int gameAreaHeight, Vector2 offset)
        {
            this.gameAreaWidth = gameAreaWidth;
            this.gameAreaHeight = gameAreaHeight;
            this.offset = offset;

            visual = new List<GameObject>();

            map = new GameObject[numberOfCellsWidth, numberOfCellsHeight];
            SizeOfCell(numberOfCellsWidth, numberOfCellsHeight);
        }
        /// <summary>
        /// Auto generates the number of cells in the game area.
        /// </summary>
        /// <param name="cellWidth"></param>
        /// <param name="cellHeight"></param>
        /// <param name="gameAreaWidth"></param>
        /// <param name="gameAreaHeight"></param>
        /// <param name="offset"></param>
        public GameMap(float cellWidth, float cellHeight, int gameAreaWidth, int gameAreaHeight, Vector2 offset)
        {
            this.gameAreaWidth = gameAreaWidth;
            this.gameAreaHeight = gameAreaHeight;
            this.cellWidth = cellWidth;
            this.cellHeight = cellHeight;
            this.offset = offset;

            map = new GameObject[(int)(gameAreaWidth / cellWidth), (int)(gameAreaHeight / cellHeight)];
        }

        /// <summary>
        /// Redefines a new width and height.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void NewGrid(int numberOfCellsWidth, int numberOfCellsHeight)
        {
            map = new GameObject[numberOfCellsWidth, numberOfCellsHeight];
            SizeOfCell(numberOfCellsWidth, numberOfCellsHeight);

            //Ensures that the content within the gameobjects fits the new grid.
            for (int i = 0; i < Gameworld.Instance.gameObjects.Count(); i++)
            {
                if (Gameworld.Instance.gameObjects[i] is ILoadable)
                    Gameworld.Instance.gameObjects[i].LoadContent(Gameworld.Instance.Content);
            }
            Gameworld.Instance.gameObjects.Clear();
            Gameworld.Instance.playerStartPosition = new Vector2(map.GetLength(0) / 2, 4);
            Gameworld.Instance.CreatePlayer();

            //New borders.
            Borders(Color.White);
        }

        public void Borders(Color color)
        {
            //Top
            GameObject go = new GameObject();
            go.AddComponent(new Spriterendere(go, "Border", 0.8f, color, true, new Vector2(gameAreaWidth, 1), true, new Rectangle(0, 0, 1, 1)));
            go.AddComponent(new Transform(go, offset, false, true));
            Gameworld.Instance.AddGameObject(go, true);
            go.LoadContent(Gameworld.Instance.Content);

            //Bottom
            GameObject go1 = new GameObject();
            go1.AddComponent(new Spriterendere(go1, "Border", 0.8f, color, true, new Vector2(gameAreaWidth, 1), true, new Rectangle(0, 0, 1, 1)));
            Vector2 temp = offset;
            temp.Y += gameAreaHeight;
            go1.AddComponent(new Transform(go1, temp, false, true));
            Gameworld.Instance.AddGameObject(go1, true);
            go1.LoadContent(Gameworld.Instance.Content);

            //Left
            GameObject go2 = new GameObject();
            go2.AddComponent(new Spriterendere(go2, "Border", 0.8f, color, true, new Vector2(1, gameAreaHeight), true, new Rectangle(0, 0, 1, 1)));
            go2.AddComponent(new Transform(go2, offset - new Vector2(1, 0), false, true));
            Gameworld.Instance.AddGameObject(go2, true);
            go2.LoadContent(Gameworld.Instance.Content);

            //Right
            GameObject go3 = new GameObject();
            go3.AddComponent(new Spriterendere(go3, "Border", 0.8f, color, true, new Vector2(1, gameAreaHeight), true, new Rectangle(0, 0, 1, 1)));
            temp = offset;
            temp.X += gameAreaWidth;
            go3.AddComponent(new Transform(go3, temp, false, true));
            Gameworld.Instance.AddGameObject(go3, true);
            go3.LoadContent(Gameworld.Instance.Content);
        }

        /// <summary>
        /// Loads the map from a GridContainer received from Server
        /// </summary>
        /// <param name="gridContainer">The Gridcongainer received from the server</param>
        public void FromContainer(Intermediate.Game.GridContainer gridContainer)
        {
            NewGrid(gridContainer.Width, gridContainer.Height);

            foreach (Intermediate.Game.GameObjectContainer gameObjectContainer in gridContainer.GameObjects)
            {
                //Please note, positions[0] is the only Global position, All other are positioned as if positions[0] is the center of the world
                Intermediate.Vector2I[] positions = gameObjectContainer.Postion;

                //Spawn gameobjects and stuff
                GameObject go = new GameObject();
                go.AddComponent(new Transform(go, positions, gameObjectContainer.Shape));
                go.AddComponent(new Spriterendere(go, "GreyToneBlock", 1));

                if (gameObjectContainer.Guid != default(Guid).ToString())
                    go.AddComponent(new NetworkController(go, new Guid(gameObjectContainer.Guid)));

                Gameworld.Instance.AddGameObject(go);
                go.LoadContent(Gameworld.Instance.Content);
            }
            List<GameObject> gos = Gameworld.Instance.gameObjects;
        }

        public Intermediate.Game.GridContainer ToContainer()
        {
            Intermediate.Game.GridContainer container = new Intermediate.Game.GridContainer(map.GetLength(0), map.GetLength(1));

            for (int i = 0; i < Gameworld.Instance.gameObjects.Count; i++)
            {
                GameObject go = Gameworld.Instance.gameObjects[i];

                if (go?.Transform?.Position == null)
                    continue;

                Vector2[] positions = new Vector2[4];
                for (int j = 0; j < positions.Count(); j++)
                    positions[j] = Gameworld.Instance.gameMap.MapPosition(go.Transform.Position[j]);
                Vector2I[] positionsI = new Vector2I[positions.Length];

                for (int j = 0; j < positions.Length; j++)
                {
                    positionsI[j] = positions[j].ToVector2I();
                }
                
                Intermediate.Game.GameObjectContainer goContainer = new Intermediate.Game.GameObjectContainer(positionsI, go.Transform.shape, go.GetComponent<NetworkController>()?.ID.ToString());

                NetworkController con;

                if (go.GetComponent<PlayerController>() != null)
                    goContainer.Guid = Gameworld.Instance.Client.Guid.ToString();
                else if ((con = go.GetComponent<NetworkController>()) != null)
                    goContainer.Guid = con.ID.ToString();
                else
                    goContainer.Guid = default(Guid).ToString();

                container.GameObjects.Add(goContainer);
            }

            return container;
        }

        /// <summary>
        /// Sets the width and height of the individual cells.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void SizeOfCell(int x, int y)
        {
            cellWidth = gameAreaWidth / x;
            cellHeight = gameAreaHeight / y;
        }

        /// <summary>
        /// Finds the world position from the map grid position.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Vector2 Position(Vector2 objPos)
        {
            float xTemp = cellWidth * objPos.X;
            float yTemp = cellHeight * objPos.Y;

            //takes into account the potential offset of the game area.
            xTemp += offset.X;
            yTemp += offset.Y;

            return new Vector2(xTemp, yTemp);
        }

        /// <summary>
        /// Finds the map grid position from it's world position.
        /// </summary>
        /// <param name="objPos"></param>
        /// <returns></returns>
        public Vector2 MapPosition(Vector2 objPos)
        {
            float xTemp = objPos.X;
            float yTemp = objPos.Y;

            //takes into account the potential offset of the game area.
            xTemp -= offset.X;
            yTemp -= offset.Y;
            //finds the map grid position.
            xTemp = (int)(xTemp / cellWidth);
            yTemp = (int)(yTemp / cellHeight);

            return new Vector2(xTemp, yTemp);
        }

        /// <summary>
        /// Checks if the given grid position is occupied.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool IsItOccupied(Vector2 pos, Vector2[] shape)
        {
            Vector2[] shapeCord = shape;
            for (int i = 0; i < shapeCord.Count(); i++)
            {
                if (map[(int)(pos.X + shapeCord[i].X), (int)(pos.Y + shapeCord[i].Y)] != null && map[(int)(pos.X + shapeCord[i].X), (int)(pos.Y + shapeCord[i].Y)].placedBlock == true)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the gameobject at the given grid position, if there is none, it returns null.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public GameObject GetObjAtPosition(Vector2 pos)
        {
            if (map[(int)(pos.X), (int)(pos.Y)] != null)
                return map[(int)(pos.X), (int)(pos.Y)];

            return null;
        }

        /// <summary>
        /// Places the given gameobject(shape) at the given grid positions.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public void PlaceGameObject(GameObject obj, Vector2 pos, Vector2[] shape)
        {
            Vector2[] shapeCord = shape;

            for (int i = 0; i < shapeCord.Count(); i++)
                map[(int)(pos.X + shapeCord[i].X), (int)(pos.Y + shapeCord[i].Y)] = obj;
        }
        /// <summary>
        /// Places the given gameobject at the given grid positions.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public void PlaceGameObject(GameObject obj, Vector2 pos)
        {
            map[(int)(pos.X), (int)(pos.Y)] = obj;
        }

        /// <summary>
        /// It removes the gameobject at the grid positions from the grid.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public void EmptyPosition(Vector2 pos, Vector2[] shape)
        {
            Vector2[] shapeCord = shape;

            for (int i = 0; i < shapeCord.Count(); i++)
                map[(int)(pos.X + shapeCord[i].X), (int)(pos.Y + shapeCord[i].Y)] = null;
        }

        /// <summary>
        /// Checks if the given grid positions is out of bound.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool IsOutOfBound(Vector2 pos, Vector2[] shape)
        {
            Vector2[] shapeCord = shape;
            int num = 0;
            for (int i = 0; i < shapeCord.Count(); i++)
                if ((map.GetLength(0) - 1) < (pos.X + shapeCord[i].X) || (map.GetLength(1) - 1) < (pos.Y + shapeCord[i].Y) || (pos.X + shapeCord[i].X) < 0 || (pos.Y + shapeCord[i].Y) < 0)
                    num++;

            if (num >= 1)
                return true;
            return false;
        }

        /// <summary>
        /// Checks if the given grid positions is such the blocks should be placed, because of blocks underneath.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool IsBlockPlaced(Vector2 pos, Vector2[] shape)
        {
            Vector2[] shapeCord = shape;

            //Checks if the blocks have hit a placed block.
            for (int i = 0; i < shapeCord.Count(); i++)
                if (map[(int)(pos.X + shapeCord[i].X), (int)(pos.Y + shapeCord[i].Y)] != null && map[(int)(pos.X + shapeCord[i].X), (int)(pos.Y + shapeCord[i].Y)].placedBlock)
                    return true;

            return false;
        }
        /// <summary>
        /// Checks if the given grid positions is at the bottom.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool IsItBottom(Vector2 pos, Vector2[] shape)
        {
            Vector2[] shapeCord = shape;

            //Checks if the blocks have hit the bottom.
            for (int i = 0; i < shapeCord.Count(); i++)
                if ((pos.Y + shapeCord[i].Y) == (map.GetLength(1) - 1))
                    return true;

            return false;
        }

        /// <summary>
        /// Place the given blocks at the given grid position.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public void PlaceBlocks(Vector2 pos, Vector2[] shape, Color color)
        {
            Vector2[] shapeCord = shape;

            //Moves blocks one down to see if there are any.
            for (int i = 0; i < shapeCord.Count(); i++)
            {
                GameObject obj = new GameObject();
                obj.AddComponent(new Spriterendere(obj, "GreyToneBlock", 1f, color));
                obj.AddComponent(new Transform(obj, new Vector2((int)(pos.X + shapeCord[i].X), (int)(pos.Y + shapeCord[i].Y)), true, false));
                obj.LoadContent(Gameworld.Instance.Content);

                Gameworld.Instance.gameObjects.Add(obj);

                map[(int)(pos.X + shapeCord[i].X), (int)(pos.Y + shapeCord[i].Y)] = obj;
            }

            //Handles if there is a complete line of blocks.
            int line = 0;
            int lineMax = map.GetLength(0);
            for (int y = 0; y < map.GetLength(1); y++)
            {
                for (int x = 0; x < map.GetLength(0); x++)
                {
                    if (map[x, y] != null && map[x, y].placedBlock == true)
                        line++;
                    if (line >= lineMax)
                        RemoveLine(y);
                }
                line = 0;
            }
        }
        /// <summary>
        /// Slave function to PlaceBlocks. This removes a line and moves all above blocks 1 line down.
        /// </summary>
        /// <param name="line"></param>
        private void RemoveLine(int line)
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                Gameworld.Instance.RemoveObject(map[i, line]);
                map[i, line] = null;
            }
            //Moves the above block one row down. 
            for (int y = line; y > 0; y--)
            {
                for (int x = 0; x < map.GetLength(0); x++)
                {
                    if (map[x, y] != null && map[x, y].placedBlock == true)
                    {
                        GameObject tempObj = map[x, y];
                        map[x, y] = null;
                        map[x, y + 1] = tempObj;
                        tempObj.GetComponent<Transform>().placedBlockPosition = Position(new Vector2(x, y + 1));
                    }
                }
            }
        }
    }
}
