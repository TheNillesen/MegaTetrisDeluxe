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
    class TextField
    {
        private int centerX, centerY;
        private Rectangle rectangle;
        private Vector2 pos;
        private Texture2D sprite;
        private string spriteName;
        private Color color;
        private SpriteFont font;
        private string text;
        private Vector2 scaleFactor;

        public SpriteFont Font { get; }

        KeyboardState keyCurrent;
        KeyboardState keyLast;

        Keys[] keysToCheck = new Keys[] {
    Keys.A, Keys.B, Keys.C, Keys.D, Keys.E,
    Keys.F, Keys.G, Keys.H, Keys.I, Keys.J,
    Keys.K, Keys.L, Keys.M, Keys.N, Keys.O,
    Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T,
    Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y,
    Keys.Z, Keys.Back, Keys.Space,
    Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.D0,
    Keys.OemComma, Keys.OemPeriod};
        KeyboardState currentKeyboardState;
        KeyboardState lastKeyboardState;

        public TextField(string spriteName, int x, int y, Vector2 scaleFactor)
        {
            this.spriteName = spriteName;
            this.centerX = x;
            this.centerY = y;
            this.scaleFactor = scaleFactor;
            color = Color.White;
            text = "";
        }

        public void LoadContent(ContentManager content)
        {
            font = content.Load<SpriteFont>("font");

            sprite = content.Load<Texture2D>(spriteName);

            rectangle = new Rectangle(0, 0, sprite.Width, sprite.Height);
        }

        public void Update()
        {
            //the key states
            keyLast = keyCurrent;
            keyCurrent = Keyboard.GetState();

            //What happens when accept button is pressed
            if (keyCurrent.IsKeyDown(Keys.Enter) && !keyLast.IsKeyDown(Keys.Enter))
                EnterButton();

            //Used to write the player name
            currentKeyboardState = Keyboard.GetState();
            foreach (Keys key in keysToCheck)
            {
                if (CheckKey(key))
                {
                    AddKeyToText(key);
                    break;
                }
            }
            lastKeyboardState = currentKeyboardState;
        }

        /// <summary>
        /// Connects to the given server. Still WIP.
        /// </summary>
        /// <returns></returns>
        public void EnterButton()
        {
            //If your connecting to a server.
            if (Gameworld.Instance.connecting)
            {
                Gameworld.Instance.IPForServerConnection = text;
               
                Gameworld.Instance.connecting = false;

                string[] values = text.Split('.');
                byte[] ipBytes = new byte[values.Length];
                for (int i = 0; i < values.Length; i++)
                    ipBytes[i] = byte.Parse(values[i]);

                //Starts game client
                Gameworld.Instance.Client = new GameClient();
                new System.Threading.Thread(() => Gameworld.Instance.Client.Connect(new System.Net.IPAddress(ipBytes), 6666)).Start();
                
                //Resets the text for later use.
                //Gameworld.Instance.IPForServerConnection = "";
                text = "";
            }

            //If your hosting a server.
            if (Gameworld.Instance.hosting)
            {
                string s = text;
                string[] values = s.Split(' ');
                int a = int.Parse(values[0]);
                int b = int.Parse(values[1]);

                //Starts server
                Gameworld.StartServer(6666, a, b, 2);
                //Starts game client
                Gameworld.Instance.Client = new GameClient();
                new System.Threading.Thread(() => Gameworld.Instance.Client.Connect(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), 6666)).Start();

                Gameworld.Instance.hosting = false;
            }

            for (int i = 0; i < Gameworld.Instance.gameObjects.Count; i++)
            {
                PlayerController pc = Gameworld.Instance.gameObjects[i].GetComponent<PlayerController>();

                if (pc != null)
                    pc.Enabled = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            pos = new Vector2(centerX - (sprite.Width * scaleFactor.X) / 2, centerY - (sprite.Height * scaleFactor.Y) / 2);
            spriteBatch.Draw(sprite, pos, rectangle, color, 0, Vector2.Zero, scaleFactor, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, text, new Vector2(centerX - text.Count() * 4, centerY), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
            if (Gameworld.Instance.connecting)
                spriteBatch.DrawString(font, "Enter host IP-address:", new Vector2(centerX - 22 * 4, centerY - 50), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
            if (Gameworld.Instance.hosting)
            {
                spriteBatch.DrawString(font, "Enter grid size consisting of two integers with a space splitting them.", new Vector2(centerX - 60 * 4, centerY - 50), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
                spriteBatch.DrawString(font, "Exameple:   12 12", new Vector2(centerX - 60 * 4, centerY - 35), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 0.9f);
            }
        }

        /// <summary>
        /// Adds the pressed keys corrosponding letter to the string text.
        /// </summary>
        /// <param name="key"></param>
        private void AddKeyToText(Keys key)
        {
            string newChar = "";

            if (text.Length >= 50 && key != Keys.Back) //The number indicates the max number of characters the string text can be.
                return;

            switch (key)
            {
                case Keys.A:
                    newChar += "a";
                    break;
                case Keys.B:
                    newChar += "b";
                    break;
                case Keys.C:
                    newChar += "c";
                    break;
                case Keys.D:
                    newChar += "d";
                    break;
                case Keys.E:
                    newChar += "e";
                    break;
                case Keys.F:
                    newChar += "f";
                    break;
                case Keys.G:
                    newChar += "g";
                    break;
                case Keys.H:
                    newChar += "h";
                    break;
                case Keys.I:
                    newChar += "i";
                    break;
                case Keys.J:
                    newChar += "j";
                    break;
                case Keys.K:
                    newChar += "k";
                    break;
                case Keys.L:
                    newChar += "l";
                    break;
                case Keys.M:
                    newChar += "m";
                    break;
                case Keys.N:
                    newChar += "n";
                    break;
                case Keys.O:
                    newChar += "o";
                    break;
                case Keys.P:
                    newChar += "p";
                    break;
                case Keys.Q:
                    newChar += "q";
                    break;
                case Keys.R:
                    newChar += "r";
                    break;
                case Keys.S:
                    newChar += "s";
                    break;
                case Keys.T:
                    newChar += "t";
                    break;
                case Keys.U:
                    newChar += "u";
                    break;
                case Keys.V:
                    newChar += "v";
                    break;
                case Keys.W:
                    newChar += "w";
                    break;
                case Keys.X:
                    newChar += "x";
                    break;
                case Keys.Y:
                    newChar += "y";
                    break;
                case Keys.Z:
                    newChar += "z";
                    break;
                case Keys.Space:
                    newChar += " ";
                    break;
                case Keys.D1:
                    newChar += "1";
                    break;
                case Keys.D2:
                    newChar += "2";
                    break;
                case Keys.D3:
                    newChar += "3";
                    break;
                case Keys.D4:
                    newChar += "4";
                    break;
                case Keys.D5:
                    newChar += "5";
                    break;
                case Keys.D6:
                    newChar += "6";
                    break;
                case Keys.D7:
                    newChar += "7";
                    break;
                case Keys.D8:
                    newChar += "8";
                    break;
                case Keys.D9:
                    newChar += "9";
                    break;
                case Keys.D0:
                    newChar += "0";
                    break;
                case Keys.OemComma:
                    newChar += ",";
                    break;
                case Keys.OemPeriod:
                    newChar += ".";
                    break;
                case Keys.Back:
                    if (text.Length != 0)
                        text = text.Remove(text.Length - 1);
                    return;
            }
            if (currentKeyboardState.IsKeyDown(Keys.RightShift) ||
                currentKeyboardState.IsKeyDown(Keys.LeftShift))
            {
                newChar = newChar.ToUpper();
            }
            text += newChar;
        }

        private bool CheckKey(Keys theKey)
        {
            return lastKeyboardState.IsKeyDown(theKey) && currentKeyboardState.IsKeyUp(theKey);
        }
    }
}
