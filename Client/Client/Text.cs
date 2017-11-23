using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using Microsoft.Xna.Framework.Content;

namespace Client
{
    class Text
    {
        private Color color;
        private SpriteFont font;
        private string text;
        private float lineSpacing;
        private Vector2 offset;

        public SpriteFont Font { get; }

        public Text(Color textColor, float lineSpacing, Vector2 offset)
        {
            this.color = textColor;
            this.lineSpacing = lineSpacing;
            this.offset = offset;
        }

        public void LoadContent(ContentManager content)
        {
            font = content.Load<SpriteFont>("font");
        }

        public void Update()
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //Key info
            Vector2 keyPos = Gameworld.Instance.gameMap.offset + new Vector2(Gameworld.Instance.gameMap.gameAreaWidth, Gameworld.Instance.gameMap.gameAreaHeight);
            keyPos += offset;
            spriteBatch.DrawString(font, "------------Key info------------", keyPos, color, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "Move left: A / left-arrow", keyPos + new Vector2(0, lineSpacing), color, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "Move right: D / right-arrow", keyPos + new Vector2(0, 2 * lineSpacing), color, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "Move down: S / down-arrow", keyPos + new Vector2(0, 3 * lineSpacing), color, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "Place now: Space", keyPos + new Vector2(0, 4 * lineSpacing), color, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "Host a game press H", keyPos + new Vector2(0, 5 * lineSpacing), color, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "Connect to a host press J", keyPos + new Vector2(0, 6 * lineSpacing), color, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "Turn music on: P", keyPos + new Vector2(0, 7 * lineSpacing), color, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "Pause music: O", keyPos + new Vector2(0, 8 * lineSpacing), color, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "Turn music down: L", keyPos + new Vector2(0, 9 * lineSpacing), color, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
            spriteBatch.DrawString(font, "Turn music Up: K", keyPos + new Vector2(0, 10 * lineSpacing), color, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
        }
    }
}
