using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;


namespace Client
{
    class Transform : Component
    {
        //Vector2 array som holder styr på brikkens positioner, position [0] er det punkt resten bevæger sig ud fra.
        public Vector2[] Position { get; set; }
        
        
        public Transform(GameObject gameObject, Vector2 position) : base(gameObject)
        {
            this.Position = new Vector2[4];
            this.Position[0] = position;
        }
        public void Translate(Vector2 translation)
        {
            Position[0] += translation;
        }
        public void MoveRight()
        {
            Gameworld.Instance.gameMap.MapPosition(Position[0]);
           
            if (Gameworld.Instance.gameMap.IsItOccupied(new Vector2 (Position[0].X + 1)) == false )
            {
                Position[0] += new Vector2(1, 0);
            }
            else
            {
                Position[0] = new Vector2(0, 0);
            }
            
        }
        public void MoveLeft()
        {
            Gameworld.Instance.gameMap.MapPosition(Position[0]);

            if (Gameworld.Instance.gameMap.IsItOccupied(new Vector2(Position[0].X - 1)) == false)
            {
                Position[0] += new Vector2(-1, 0);
            }
            else
            {
                Position[0] = new Vector2(0, 0);
            }
        }
     
    }
}
