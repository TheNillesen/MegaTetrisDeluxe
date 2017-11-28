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
        private bool customScaleFactor;
        private bool customSourceRectangle;
        public Vector2 scaleFactor;
        public Vector2 Offset { get; set; }
        public Color color;

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
            color = Color.Red;
            customScaleFactor = false;
            customSourceRectangle = false;
        }
        public Spriterendere(GameObject gameObject, string spriteName, float layerDepth, Color color) : base(gameObject)
        {
            this.spriteName = spriteName;
            this.layerDepth = layerDepth;
            this.color = color;
            customScaleFactor = false;
            customSourceRectangle = false;
        }
        public Spriterendere(GameObject gameObject, string spriteName, float layerDepth, Color color, bool myCustomScaleFactor, Vector2 scaleFactor, bool myCustomSourceRectangle, Rectangle sourceRectangle) : base(gameObject)
        {
            this.spriteName = spriteName;
            this.layerDepth = layerDepth;
            this.scaleFactor = scaleFactor;
            this.color = color;
            this.rectangle = sourceRectangle;
            if(myCustomScaleFactor)
                customScaleFactor = true;
            if (myCustomSourceRectangle)
                customSourceRectangle = true;
        }

        public void update()
        {

        }

        public void LoadContent(ContentManager content)
        {
            sprite = content.Load<Texture2D>(spriteName);
            if (!customScaleFactor)
            {
                scaleFactor.X = Gameworld.Instance.gameMap.cellWidth / sprite.Width;
                scaleFactor.Y = Gameworld.Instance.gameMap.cellHeight / sprite.Height;
            }
            if(!customSourceRectangle)
                rectangle = new Rectangle(0, 0, sprite.Width, sprite.Height);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            //Checks if the block is placed or not..
            if (!gameObject.placedBlock)
            {
                for (int i = 0; i < gameObject.Transform.Position.Count(); i++)
                    spriteBatch.Draw(sprite, gameObject.Transform.Position[i], Rectangle, color, 0f, Vector2.Zero, scaleFactor, SpriteEffects.None, layerDepth);
            }
            if(gameObject.placedBlock)
                spriteBatch.Draw(sprite, gameObject.Transform.placedBlockPosition, Rectangle, color, 0f, Vector2.Zero, scaleFactor, SpriteEffects.None, layerDepth);
        }
    }
}

