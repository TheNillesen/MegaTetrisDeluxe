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
        private Vector2 offset;     //The game areas position in the window.
        private int gameAreaWidth;  //The area width in which the game it self takes place.
        private int gameAreaHeight; //The area height in which the game it self takes place.

        public GameObject[,] map; //The map grid.
        public float cellWidth;   //The width of a cell.
        public float cellHeight;  //The height of a cell.

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
        /// Loads the map from a GridContainer received from Server
        /// </summary>
        /// <param name="gridContainer">The Gridcongainer received from the server</param>
        public void FromContainer(Intermediate.Game.GridContainer gridContainer)
        {
            this.gameAreaWidth = gridContainer.Width;
            this.gameAreaHeight = gridContainer.Height;

            foreach(Intermediate.Game.GameObjectContainer gameObjectContainer in gridContainer.GameObjects)
            {
                //Please note, positions[0] is the only Global position, All other are positioned as if positions[0] is the center of the world
                Intermediate.Vector2I[] positions = Intermediate.GameShapeHelper.GetShape(gameObjectContainer.Shape, gameObjectContainer.Postion);

                //Spawn gameobjects and stuff
            }
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
            int num = 0;
            for (int i = 0; i < shapeCord.Count(); i++)
            {
                if (new Vector2(pos.X + shapeCord[i].X, pos.Y + shapeCord[i].Y) == null)
                    num++;
            }
            if (num >= 4)
                return true;
            return false;
        }

        /// <summary>
        /// Returns the gameobject at the given grid position, if there is none, it returns null.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public GameObject GetObjAtPosition(Vector2 pos)
        {
            if(map[(int)(pos.X), (int)(pos.Y)] != null)
                return map[(int)(pos.X), (int)(pos.Y)];

            return null;
        }

        /// <summary>
        /// Places the given gameobject at the given grid positions.
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

            ////Moves blocks one down to see if there are any.
            //for(int i = 0; i < shapeCord.Count(); i++)
            //    shapeCord[i] += new Vector2(0, 1);

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
                obj.AddComponent(new Transform(obj, new Vector2((int)(pos.X + shapeCord[i].X), (int)(pos.Y + shapeCord[i].Y)), true));
                obj.LoadContent(Gameworld.Instance.Content);

                Gameworld.Instance.gameObjects.Add(obj);

                map[(int)(pos.X + shapeCord[i].X), (int)(pos.Y + shapeCord[i].Y)] = obj;
            }
        }
    }
}
