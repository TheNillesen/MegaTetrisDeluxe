using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Spriterendere
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
        {
            spriteBatch.Draw(sprite, GetGameObject().Transform.Position + Offset, Rectangle, Color.White);
        }

    }




}

