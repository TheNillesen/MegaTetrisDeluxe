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
    class Transform : Component, ITickable
    {
        //Vector2 array som holder styr på brikkens positioner, position [0] er det punkt resten bevæger sig ud fra.
        public Vector2[] Position { get; set; }
        // Shapes: I, L, Lightning, Lightning_invers, L_invers, Square, T, Polygon
        public GameShapes shape;
        public Vector2[] ShapeCord;

        private float angle;

        public Transform(GameObject gameObject, Vector2 position) : base(gameObject)
        {
            this.Position = new Vector2[4];

            //For test, I have given a standard shape
            Shape();
            angle = 0f;
            //The shape's coordinates
            Vector2I[] tempShapeCord = GameShapeHelper.GetShape(shape);
            ShapeCord = new Vector2[4];
            for(int i = 0; i < ShapeCord.Count(); i++)
                ShapeCord[i] = new Vector2(tempShapeCord[i].X, tempShapeCord[i].Y);

            Vector2 tempPos = Gameworld.Instance.gameMap.MapPosition(position);
            for (int i = 0; i < Position.Count(); i++)
                Position[i] = Gameworld.Instance.gameMap.Position(new Vector2(tempPos.X + ShapeCord[i].X, tempPos.Y + ShapeCord[i].Y));
        }
        public void Translate(Vector2 translation)
        {
            Position[0] += translation;
        }
        //function that checks the current gameobject position and if the tile on the right side is occupied or not, if its not , we move our gameobject to the new position and 
        //sets the new position as occupied while the old tile loses its occupied status. same goes for the MoveLeft() fuction further down..
        /// <summary>
        /// moves the character right
        /// </summary>
        public void MoveRight()
        {
            Move(new Vector2(1, 0));
        }
        /// <summary>
        /// moves the character left
        /// </summary>
        public void MoveLeft()
        {
            Move(new Vector2(-1, 0));
        }
        /// <summary>
        /// moves the character one tile down.
        /// </summary>
        public void MoveDown()
        {
            Move(new Vector2(0, 1));
        }

        private void Move(Vector2 move)
        {
            Vector2 tempPos = Gameworld.Instance.gameMap.MapPosition(Position[0]);

            if (!Gameworld.Instance.gameMap.IsOutOfBound(new Vector2(tempPos.X, tempPos.Y) + move, ShapeCord) && Gameworld.Instance.gameMap.IsItOccupied(new Vector2(tempPos.X, tempPos.Y) + move, ShapeCord) == false)
            {
                Gameworld.Instance.gameMap.EmptyPosition(tempPos, ShapeCord);
                tempPos += move;
                Gameworld.Instance.gameMap.PlaceGameObject(gameObject, tempPos, ShapeCord);
                for (int i = 0; i < Position.Count(); i++)
                    Position[i] = Gameworld.Instance.gameMap.Position(new Vector2(tempPos.X + ShapeCord[i].X, tempPos.Y + ShapeCord[i].Y));
            }
        }

        /// <summary>
        /// Roteates the character 90 degrees left.
        /// </summary>
        public void RotateLeft()
        {
            Rotate(-90);
        }

        /// <summary>
        /// Roteates the character 90 degrees right.
        /// </summary>
        public void RotateRight()
        {
            Rotate(90);
        }

        private void Rotate(float rotationAngle)
        {
            Vector2[] temp = ShapeCord;
            for (int i = 1; i < temp.Count(); i++)
                temp[i] = RotPointsAroundPointMath.RotatePoint(temp[i], temp[0], rotationAngle);

            Vector2 tempPos = Gameworld.Instance.gameMap.MapPosition(Position[0]);

            if (!Gameworld.Instance.gameMap.IsOutOfBound(tempPos, temp) && Gameworld.Instance.gameMap.IsItOccupied(tempPos, temp) == false)
            {
                ShapeCord = temp;
                Gameworld.Instance.gameMap.EmptyPosition(tempPos, ShapeCord);
                Gameworld.Instance.gameMap.PlaceGameObject(gameObject, tempPos, ShapeCord);
                for (int i = 0; i < Position.Count(); i++)
                    Position[i] = Gameworld.Instance.gameMap.Position(new Vector2(tempPos.X + ShapeCord[i].X, tempPos.Y + ShapeCord[i].Y));
            }
        }

        /// <summary>
        /// Sets a random shape.
        /// </summary>
        public void Shape()
        {
            Random rnd = new Random();
            int num = rnd.Next(1, 8);
            //Their are 7 shapes in total.
            switch (num)
            {
                case 1:
                    shape = GameShapes.I;
                    break;
                case 2:
                    shape = GameShapes.L;
                    break;
                case 3:
                    shape = GameShapes.Lightning;
                    break;
                case 4:
                    shape = GameShapes.Lightning_Inverse;
                    break;
                case 5:
                    shape = GameShapes.L_Inverse;
                    break;
                case 6:
                    shape = GameShapes.Square;
                    break;
                case 7:
                    shape = GameShapes.T;
                    break;
                default:
                    shape = GameShapes.I;
                    break;
            }

            //The shape's coordinates
            Vector2I[] tempShapeCord = GameShapeHelper.GetShape(shape);
            ShapeCord = new Vector2[4];
            for (int i = 0; i < ShapeCord.Count(); i++)
                ShapeCord[i] = new Vector2(tempShapeCord[i].X, tempShapeCord[i].Y);
        }

        public void OnTick()
        {
            MoveDown();
        }
     
    }
}
