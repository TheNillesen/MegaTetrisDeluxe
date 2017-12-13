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
        public Vector2 placedBlockPosition;
        // Shapes: I, L, Lightning, Lightning_invers, L_invers, Square, T, Polygon
        public GameShapes shape;
        public Vector2[] ShapeCord;

        private float angle;
        private Random rnd;

        public Transform(GameObject gameObject, Vector2 position) : base(gameObject)
        {
            Slave(position);
        }
        public Transform(GameObject gameObject, Vector2I position, GameShapes shape)
            : base(gameObject)
        {
            Vector2I[] tempPos = GameShapeHelper.GetShape(shape, position);
            ShapeCord = new Vector2[tempPos.Length];

            Position = new Vector2[tempPos.Length];

            Vector2I[] tempShapeCord = GameShapeHelper.GetShape(shape);
            for (int i = 0; i < tempShapeCord.Count(); i++)
                ShapeCord[i] = new Vector2(tempShapeCord[i].X, tempShapeCord[i].Y);

            for (int i = 0; i < tempPos.Length; i++)
            {
                Position[i] = tempPos[i].ToVector2();
            }

            SColor(shape);
            this.shape = shape;
            rnd = new Random();
        }
        /// <summary>
        /// If it's a block on the map set placedblock = true. If it's other such as the border set other = true.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="position"></param>
        /// <param name="placedBlock"></param>
        public Transform(GameObject gameObject, Vector2 position, bool placedBlock, bool other) : base(gameObject)
        {
            if (placedBlock)
            {
                placedBlockPosition = Gameworld.Instance.gameMap.Position(position);
                gameObject.placedBlock = true;
            }
            else if (other)
            {
                placedBlockPosition = position;
                gameObject.placedBlock = true;
            }
            else
            {
                Slave(position);
            }
        }

        public Transform(GameObject gameObject, Vector2I[] positions, GameShapes shape) : base(gameObject)
        {
            this.Position = new Vector2[positions.Length];
            this.ShapeCord = new Vector2[positions.Length];
            Vector2[] temp = new Vector2[positions.Length];

            for (int i = 0; i < positions.Length; i++)
            {
                Position[i] = positions[i].ToVector2();
                temp[i] = Gameworld.Instance.gameMap.MapPosition(positions[i].ToVector2());
                ShapeCord[i] = temp[i] - temp[0];
            }

            SColor(shape);
            this.shape = shape;
            rnd = new Random();
        }

        /// <summary>
        /// Slave function to the constructors.
        /// </summary>
        /// <param name="position"></param>
        private void Slave(Vector2 position)
        {
            this.Position = new Vector2[4];
            rnd = new Random();
            ShapeCord = new Vector2[4];

            //For test, I have given a standard shape
            ShapeAndColor();
            angle = 0f;
            //The shape's coordinates
            Vector2I[] tempShapeCord = GameShapeHelper.GetShape(shape);
            ShapeCord = new Vector2[4];
            for (int i = 0; i < ShapeCord.Count(); i++)
                ShapeCord[i] = new Vector2(tempShapeCord[i].X, tempShapeCord[i].Y);

            for (int i = 0; i < Position.Count(); i++)
                Position[i] = Gameworld.Instance.gameMap.Position(new Vector2(position.X + ShapeCord[i].X, position.Y + ShapeCord[i].Y));
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
        public void MoveDown(bool enforced = false)
        {
            if (Position != null)
            {
                if (!enforced)
                {
                    BlockPlaceCheck();
                }
                Move(new Vector2(0, 1));
            }
        }

        private void Move(Vector2 move)
        {
            Vector2 tempPos = Gameworld.Instance.gameMap.MapPosition(Position[0]);

            if (!Gameworld.Instance.gameMap.IsOutOfBound(tempPos + move, ShapeCord)
                && Gameworld.Instance.gameMap.IsItOccupied(tempPos + move, ShapeCord) == false)
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

            if (!Gameworld.Instance.gameMap.IsOutOfBound(tempPos, temp)
                && Gameworld.Instance.gameMap.IsItOccupied(tempPos, temp) == false)
            {
                ShapeCord = temp;
                Gameworld.Instance.gameMap.EmptyPosition(tempPos, ShapeCord);
                Gameworld.Instance.gameMap.PlaceGameObject(gameObject, tempPos, ShapeCord);
                for (int i = 0; i < Position.Count(); i++)
                    Position[i] = Gameworld.Instance.gameMap.Position(new Vector2(tempPos.X + ShapeCord[i].X, tempPos.Y + ShapeCord[i].Y));
                angle += rotationAngle;
                if (angle == 360)
                    angle = 0;
            }
        }

        /// <summary>
        /// Checks if the blocks shuold be placed and if so, places the blocks and gives the player a new block.
        /// </summary>
        private void BlockPlaceCheck()
        {
            if (Position == null) return;

            bool placed = false;
            Vector2 tempPos = Gameworld.Instance.gameMap.MapPosition(Position[0]);
            //We move the blocks one down, as we need to check the next position befor we move to it
            tempPos += new Vector2(0, 1);

            //Checks if the blocks should be placed.
            if (Gameworld.Instance.gameMap.IsBlockPlaced(tempPos, ShapeCord))
            {
                //Since there is blocks at the new position, then we move the blocks one tick up.
                tempPos += new Vector2(0, -1);
                //Places the blocks.
                Gameworld.Instance.gameMap.PlaceBlocks(tempPos, ShapeCord, gameObject.GetComponent<Spriterendere>().color);

                placed = true;
            }
            //Checks if the blocks have hit the bottom and should be placed.
            if (Gameworld.Instance.gameMap.IsItBottom(tempPos, ShapeCord))
            {
                //Places the blocks.
                Gameworld.Instance.gameMap.PlaceBlocks(tempPos, ShapeCord, gameObject.GetComponent<Spriterendere>().color);
                placed = true;
            }
            //If the blocks do get placed
            if (placed)
            {
                //New shape and color.
                if(this.gameObject.GetComponent<PlayerController>() != null)
                    ShapeAndColor();

                //Moves back to start position.
                Vector2 newPos = Gameworld.Instance.playerStartPosition;
                for (int i = 0; i < Position.Count(); i++)
                    Position[i] = Gameworld.Instance.gameMap.Position(new Vector2(newPos.X + ShapeCord[i].X, newPos.Y + ShapeCord[i].Y));

                if (Gameworld.Instance.gameMap.IsBlockPlaced(newPos, ShapeCord) == true)
                {
                    Gameworld.Instance.Exit();
                }
            }

        }

        public void PlaceBlockNow()
        {
            Vector2 move = new Vector2(0, 1);

            Vector2 tempPos = Gameworld.Instance.gameMap.MapPosition(Position[0]);

            for (int i = 0; i < Gameworld.Instance.gameMap.map.GetLength(0); i++)
            {
                move = new Vector2(0, 1 + i);

                if (Gameworld.Instance.gameMap.IsOutOfBound(tempPos + move, ShapeCord))
                {
                    tempPos += move - new Vector2(0, 2);
                    for (int j = 0; j < Position.Count(); j++)
                        Position[j] = Gameworld.Instance.gameMap.Position(new Vector2(tempPos.X + ShapeCord[j].X, tempPos.Y + ShapeCord[j].Y));
                    BlockPlaceCheck();
                    return;
                }
                if (Gameworld.Instance.gameMap.IsItOccupied(tempPos + move, ShapeCord) == true)
                {
                    tempPos += move - new Vector2(0, 1);
                    for (int j = 0; j < Position.Count(); j++)
                        Position[j] = Gameworld.Instance.gameMap.Position(new Vector2(tempPos.X + ShapeCord[j].X, tempPos.Y + ShapeCord[j].Y));
                    BlockPlaceCheck();
                    return;
                }
            }
        }

        /// <summary>
        /// Sets a random shape.
        /// </summary>
        public void ShapeAndColor()
        {
            int num = rnd.Next(1, 8);
            //Their are 7 shapes in total.
            switch (num)
            {
                case 1:
                    shape = GameShapes.I;
                    gameObject.GetComponent<Spriterendere>().color = Color.Red;
                    break;
                case 2:
                    shape = GameShapes.L;
                    gameObject.GetComponent<Spriterendere>().color = Color.Pink;
                    break;
                case 3:
                    shape = GameShapes.Lightning;
                    gameObject.GetComponent<Spriterendere>().color = Color.Green;
                    break;
                case 4:
                    shape = GameShapes.Lightning_Inverse;
                    gameObject.GetComponent<Spriterendere>().color = Color.LightBlue;
                    break;
                case 5:
                    shape = GameShapes.L_Inverse;
                    gameObject.GetComponent<Spriterendere>().color = Color.White;
                    break;
                case 6:
                    shape = GameShapes.Square;
                    gameObject.GetComponent<Spriterendere>().color = Color.Blue;
                    break;
                case 7:
                    shape = GameShapes.T;
                    gameObject.GetComponent<Spriterendere>().color = Color.Brown;
                    break;
                default:
                    shape = GameShapes.I;
                    gameObject.GetComponent<Spriterendere>().color = Color.Red;
                    break;
            }

            Vector2I[] tempShapeCord = GameShapeHelper.GetShape(shape);
            for (int i = 0; i < tempShapeCord.Count(); i++)
                ShapeCord[i] = new Vector2(tempShapeCord[i].X, tempShapeCord[i].Y);

            //Sends the new shape to all other players in the game.
            if (Gameworld.Instance.Client != null)
                Gameworld.Instance.Client.Send(new NetworkPacket("Shape", null, shape));
        }

        /// <summary>
        /// Get shape's color.
        /// </summary>
        private void SColor(GameShapes shape)
            {
                switch (shape)
                {
                    case GameShapes.I:
                        gameObject.GetComponent<Spriterendere>().color = Color.Red;
                        break;
                    case GameShapes.L:
                        gameObject.GetComponent<Spriterendere>().color = Color.Pink;
                        break;
                    case GameShapes.Lightning:
                        gameObject.GetComponent<Spriterendere>().color = Color.Green;
                        break;
                    case GameShapes.Lightning_Inverse:
                        gameObject.GetComponent<Spriterendere>().color = Color.LightBlue;
                        break;
                    case GameShapes.L_Inverse:
                        gameObject.GetComponent<Spriterendere>().color = Color.White;
                        break;
                    case GameShapes.Square:
                        gameObject.GetComponent<Spriterendere>().color = Color.Blue;
                        break;
                    case GameShapes.T:
                        gameObject.GetComponent<Spriterendere>().color = Color.Brown;
                        break;
                    default:
                        gameObject.GetComponent<Spriterendere>().color = Color.Red;
                        break;
                }
        }

        /// <summary>
        /// Changes the shape to the given.
        /// </summary>
        /// <param name="shape"></param>
        public void NewShape(GameShapes shape)
        {
            Vector2I[] tempShapeCord = GameShapeHelper.GetShape(shape);
            for (int i = 0; i < tempShapeCord.Length; i++)
                ShapeCord[i] = new Vector2(tempShapeCord[i].X, tempShapeCord[i].Y);

            Vector2 tempPos = Gameworld.Instance.gameMap.MapPosition(Position[0]);
            for (int i = 0; i < Position.Length; i++)
                Position[i] = Gameworld.Instance.gameMap.Position(tempPos + ShapeCord[i]);

            this.shape = shape;

            SColor(shape);
        }

        public void OnTick()
            {
                MoveDown();
            }
        
    }
}
