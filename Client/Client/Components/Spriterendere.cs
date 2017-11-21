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
        private Vector2 scaleFactor;
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
            scaleFactor.X = Gameworld.Instance.gameMap.cellWidth / sprite.Width;
            scaleFactor.Y = Gameworld.Instance.gameMap.cellHeight / sprite.Height;
            rectangle = new Rectangle(0, 0, sprite.Width, sprite.Height);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            for(int i = 0; i < gameObject.Transform.Position.Count(); i++)
                spriteBatch.Draw(sprite, gameObject.Transform.Position[i], Rectangle, Color.White, 0f, Vector2.Zero, scaleFactor, SpriteEffects.None, layerDepth);
        }
    }
}

