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
        public Vector2 Position { get; set; }

        public Transform(GameObject gameObject, Vector2 position) : base(gameObject)
        {
            this.Position = position;
        }
        public void Translate(Vector2 translation)
        {
            Position += translation;
        }
    }
}
