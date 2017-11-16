using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Client
{
    class GameMap
    {
        private Vector2 offset; //The game areas position in the window.
        private int gameAreaWidth;
        private int gameAreaHeight;

        public GameObject[,] map;
        public float cellWidth;
        public float cellHeight;

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
        /// Finds the width and height of the individual cells.
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
        /// <param name="objPos"></param>
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
        /// Moves the given GameObject from position a to b. Still WIP.
        /// </summary>
        public void MovePosition(Vector2 posA, Vector2 posB)
        {
            GameObject temp;
            temp = map[(int)posA.X, (int)posA.Y];
            map[(int)posA.X, (int)posA.Y] = null;
            map[(int)posB.X, (int)posB.Y] = temp;
        }
    }
}
