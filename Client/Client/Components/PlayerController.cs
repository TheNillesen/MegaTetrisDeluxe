using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Intermediate;

namespace Client
{
    class PlayerController : Component, IUpdatable
    {
        KeyboardState keyCurrent;
        KeyboardState keyLast;
        SFX sfxHandler;

        public PlayerController(GameObject gameObject) : base(gameObject)
        {
            keyLast = Keyboard.GetState();
            keyLast = Keyboard.GetState();
            sfxHandler = new SFX();
        }

        public void Update()
        {
            //the key states
            keyLast = keyCurrent;
            keyCurrent = Keyboard.GetState();

            //Handles the players input
            if (keyCurrent.IsKeyDown(Keys.Right) && !keyLast.IsKeyDown(Keys.Right) || keyCurrent.IsKeyDown(Keys.D) && !keyLast.IsKeyDown(Keys.D))
            {
                if(Gameworld.Instance.Client != null)
                    Gameworld.Instance.Client.Send(new NetworkPacket("Move", null, new Vector2(1, 0).ToVector2I()));
                gameObject.Transform.MoveRight();
            }
            if (keyCurrent.IsKeyDown(Keys.Left) && !keyLast.IsKeyDown(Keys.Left) || keyCurrent.IsKeyDown(Keys.A) && !keyLast.IsKeyDown(Keys.A))
            {
                if (Gameworld.Instance.Client != null)
                    Gameworld.Instance.Client.Send(new NetworkPacket("Move", null, new Vector2(-1, 0).ToVector2I()));
                gameObject.Transform.MoveLeft();
            }
            if (keyCurrent.IsKeyDown(Keys.Down) && !keyLast.IsKeyDown(Keys.Down) || keyCurrent.IsKeyDown(Keys.S) && !keyLast.IsKeyDown(Keys.S))
            {
                if (Gameworld.Instance.Client != null)
                    Gameworld.Instance.Client.Send(new NetworkPacket("Move", null, new Vector2(0, 1).ToVector2I()));
                gameObject.Transform.MoveDown();
            }
            if (keyCurrent.IsKeyDown(Keys.Q) && !keyLast.IsKeyDown(Keys.Q))
            {
                if (Gameworld.Instance.Client != null)
                    Gameworld.Instance.Client.Send(new NetworkPacket("Rotate", null, false));
                gameObject.Transform.RotateLeft();
            }
            if (keyCurrent.IsKeyDown(Keys.E) && !keyLast.IsKeyDown(Keys.E))
            {
                if (Gameworld.Instance.Client != null)
                    Gameworld.Instance.Client.Send(new NetworkPacket("Rotate", null, true));
                gameObject.Transform.RotateRight();
            }
            if (keyCurrent.IsKeyDown(Keys.Space) && !keyLast.IsKeyDown(Keys.Space))
            {
                if (Gameworld.Instance.Client != null)
                    Gameworld.Instance.Client.Send(new NetworkPacket("PBN", null));
                gameObject.Transform.PlaceBlockNow();
            }
            //Connect to server
            if (Gameworld.Instance.connecting == false && keyCurrent.IsKeyDown(Keys.J) && !keyLast.IsKeyDown(Keys.J))
            {
                Enabled = false;
                Gameworld.Instance.connecting = true;
            }
            //Host server
            if (Gameworld.Instance.hosting == false && keyCurrent.IsKeyDown(Keys.H) && !keyLast.IsKeyDown(Keys.H))
            {
                Enabled = false;
                Gameworld.Instance.hosting = true;
            }
            //music controls
            if (keyCurrent.IsKeyDown(Keys.P))
            {
                sfxHandler.PlayMusic();
            }
            if (keyCurrent.IsKeyDown(Keys.O))
            {
                sfxHandler.PauseMusic();
            }
            if (keyCurrent.IsKeyDown(Keys.K))
            {
                sfxHandler.MusicVolumeUp();
            }
            if (keyCurrent.IsKeyDown(Keys.L))
            {
                sfxHandler.MusicVolumeDown();
            }
        }
    }
}
