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
    class Spriterendere : Component, IDrawable, ILoadable
    {
        private Rectangle rectangle;
        private Texture2D sprite;
        private string spriteName;
        private float layerDepth;
        public Vector2 Offset { get; set; }

        public Rectangle Rectangle
        {
            get
            {
                return rectangle;
            }

            set
            {
                rectangle = value;
            }
        }

        public Texture2D Sprite
        {
            get
            {
                return sprite;
            }

            set
            {
                sprite = value;
            }
        }

        public Spriterendere(GameObject gameObject, string spriteName, float layerDepth) : base(gameObject)
        {
            this.spriteName = spriteName;
            this.layerDepth = layerDepth;

        }

        public void update()
        {

        }

        public void LoadContent(ContentManager content)
        {
            sprite = content.Load<Texture2D>(spriteName);

        }
        public void Draw(SpriteBatch spriteBatch)
        {//Er ikke helt færdig den skal tage fat i gameobject.transfrom når transform er færdig.
            spriteBatch.Draw(sprite, gameObject.GetComponent<Transform>().Position[0], /*gameObject.Transform.Position[0]*/ Rectangle, Color.White);
        }

    }




}

