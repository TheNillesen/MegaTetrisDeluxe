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

        public PlayerController(GameObject gameObject) : base(gameObject)
        {
            keyLast = Keyboard.GetState();
            keyLast = Keyboard.GetState();
        }

        public void Update()
        {
            keyLast = keyCurrent;
            keyCurrent = Keyboard.GetState();

            if (keyCurrent.IsKeyDown(Keys.Right) && !keyLast.IsKeyDown(Keys.Right))
                gameObject.Transform.MoveRight();
            if (keyCurrent.IsKeyDown(Keys.Left) && !keyLast.IsKeyDown(Keys.Left))
                gameObject.Transform.MoveLeft();
            if (keyCurrent.IsKeyDown(Keys.Space) && !keyLast.IsKeyDown(Keys.Space))
            {

            }
        }
    }
}
