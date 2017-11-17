using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Intermediate;

namespace Client
{
    class Transform : Component
    {
        //Vector2 array som holder styr på brikkens positioner, position [0] er det punkt resten bevæger sig ud fra.
        public Vector2[] Position { get; set; }
        // Shapes: I, L, Lightning, Lightning_invers, L_invers, Square, T, Polygon
        public GameShapes shape;
        public Vector2I[] ShapeCord
        {
            get
            {
                return GameShapeHelper.GetShape(shape);
            }
        }

        public Transform(GameObject gameObject, Vector2 position) : base(gameObject)
        {
            this.Position = new Vector2[4];

            Vector2 tempPos = Gameworld.Instance.gameMap.MapPosition(position);
            for (int i = 0; i < Position.Count(); i++)
                Position[i] = Gameworld.Instance.gameMap.Position(new Vector2(tempPos.X + ShapeCord[i].X, tempPos.Y + ShapeCord[i].Y));


            //For test, I have given a standard shape
            shape = GameShapes.Lightning;
        }
        public void Translate(Vector2 translation)
        {
            Position[0] += translation;
        }
        //function that checks the current gameobject position and if the tile on the right side is occupied or not, if its not , we move our gameobject to the new position and 
        //sets the new position as occupied while the old tile loses its occupied status. same goes for the MoveLeft() fuction further down.
        /// <summary>
        /// moves the character right
        /// </summary>
        public void MoveRight()
        {
            Vector2 tempPos = Gameworld.Instance.gameMap.MapPosition(Position[0]);

            if (!Gameworld.Instance.gameMap.IsOutOfBound(new Vector2(tempPos.X + 1, tempPos.Y), shape) && Gameworld.Instance.gameMap.IsItOccupied(new Vector2 (tempPos.X + 1, tempPos.Y), shape) == false)
            {
                Gameworld.Instance.gameMap.EmptyPosition(tempPos, shape);
                tempPos += new Vector2(1, 0);
                Gameworld.Instance.gameMap.PlaceGameObject(gameObject, tempPos, shape);
                for(int i = 0; i < Position.Count(); i++)
                    Position[i] = Gameworld.Instance.gameMap.Position(new Vector2(tempPos.X + ShapeCord[i].X, tempPos.Y + ShapeCord[i].Y));
            }
        }
        /// <summary>
        /// moves the character left
        /// </summary>
        public void MoveLeft()
        {
            Vector2 tempPos = Gameworld.Instance.gameMap.MapPosition(Position[0]);

                if (!Gameworld.Instance.gameMap.IsOutOfBound(new Vector2(tempPos.X - 1, tempPos.Y), shape) && Gameworld.Instance.gameMap.IsItOccupied(new Vector2(tempPos.X - 1, tempPos.Y), shape) == false)
                {
                    Gameworld.Instance.gameMap.EmptyPosition(tempPos, shape);
                    tempPos += new Vector2(-1, 0);
                    Gameworld.Instance.gameMap.PlaceGameObject(gameObject, tempPos, shape);
                    for (int i = 0; i < Position.Count(); i++)
                        Position[i] = Gameworld.Instance.gameMap.Position(new Vector2(tempPos.X + ShapeCord[i].X, tempPos.Y + ShapeCord[i].Y));
            }
        }
        /// <summary>
        /// moves the character one tile down. not finished
        /// </summary>
        public void MoveDown()
        {
            Vector2 tempPos = Gameworld.Instance.gameMap.MapPosition(Position[0]);

            if (Gameworld.Instance.gameMap.IsItOccupied(new Vector2(tempPos.X, tempPos.Y + 1), shape) == false)
            {
                Gameworld.Instance.gameMap.EmptyPosition(tempPos, shape);
                tempPos += new Vector2(0, 0);
                Gameworld.Instance.gameMap.PlaceGameObject(gameObject, tempPos, shape);
            }
        }
     
    }
}
