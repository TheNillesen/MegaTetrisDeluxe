using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intermediate;
using Microsoft.Xna.Framework;

namespace Intermediate
{
    static class Vector2Extension
    {
        public static Vector2I ToVector2I(this Vector2 vector)
        {
            return new Vector2I((int)vector.X, (int)vector.Y);
        }

        public static Vector2 ToVector2(this Vector2I vector)
        {
            return new Vector2(vector.X, vector.Y);
        }
    }
}
