using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

            //Host server
            if (keyCurrent.IsKeyDown(Keys.H) && !keyLast.IsKeyDown(Keys.H))
            {
                //Starts server
                Gameworld.startServer(6666, Gameworld.Instance.gameMap.map.GetLength(0), Gameworld.Instance.gameMap.map.GetLength(1), 2);
                //Starts game client
                GameClient gc = new GameClient();
                new System.Threading.Thread(() => gc.Connect(new System.Net.IPAddress(new byte[] { 127, 0, 0, 1 }), 6666)).Start();
            }

            //Handles the players input
            if (keyCurrent.IsKeyDown(Keys.Right) && !keyLast.IsKeyDown(Keys.Right) || keyCurrent.IsKeyDown(Keys.D) && !keyLast.IsKeyDown(Keys.D))
                gameObject.Transform.MoveRight();
            if (keyCurrent.IsKeyDown(Keys.Left) && !keyLast.IsKeyDown(Keys.Left) || keyCurrent.IsKeyDown(Keys.A) && !keyLast.IsKeyDown(Keys.A))
                gameObject.Transform.MoveLeft();
            if (keyCurrent.IsKeyDown(Keys.Down) && !keyLast.IsKeyDown(Keys.Down) || keyCurrent.IsKeyDown(Keys.S) && !keyLast.IsKeyDown(Keys.S))
                gameObject.Transform.MoveDown();
            if (keyCurrent.IsKeyDown(Keys.Q) && !keyLast.IsKeyDown(Keys.Q))
                gameObject.Transform.RotateLeft();
            if (keyCurrent.IsKeyDown(Keys.E) && !keyLast.IsKeyDown(Keys.E))
                gameObject.Transform.RotateRight();
            if (keyCurrent.IsKeyDown(Keys.Space) && !keyLast.IsKeyDown(Keys.Space))
                gameObject.Transform.PlaceBlockNow();

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
