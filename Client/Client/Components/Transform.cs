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
            Vector2 translation = Vector2.Zero;
            translation += new Vector2(1, 0);
            if(gameMap.)
        }
        public void MoveLeft()
        {
            Vector2 translation = Vector2.Zero;
            translation += new Vector2(-1, 0);
        }
     
    }
}
