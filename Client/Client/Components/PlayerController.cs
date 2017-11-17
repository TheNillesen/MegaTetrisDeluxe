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


            //if (state.IsKeyDown(Keys.Right))
            //    gameObject.Transform.MoveRight();
            //if (state.IsKeyDown(Keys.Left))
            //    gameObject.Transform.MoveLeft();
        }
    }
}
