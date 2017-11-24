using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intermediate
{
    public static class GameShapeHelper
    {
        public static Vector2I[] GetShape(GameShapes shape)
        {
            switch(shape)
            {
                case GameShapes.I:
                    return new Vector2I[] { new Vector2I(0, 0), new Vector2I(0, 1), new Vector2I(0, -1), new Vector2I(0, -2) };
                case GameShapes.L:
                    return new Vector2I[] { new Vector2I(0, 0), new Vector2I(0, 1), new Vector2I(0, -1), new Vector2I(-1, -1) };
                case GameShapes.Lightning:
                    return new Vector2I[] { new Vector2I(0, 0), new Vector2I(1, 0), new Vector2I(1, 1), new Vector2I(0, -1) };
                case GameShapes.Lightning_Inverse:
                    return new Vector2I[] { new Vector2I(0, 0), new Vector2I(-1, 0), new Vector2I(-1, 1), new Vector2I(0, -1) };
                case GameShapes.L_Inverse:
                    return new Vector2I[] { new Vector2I(0, 0), new Vector2I(0, 1), new Vector2I(0, -1), new Vector2I(1, -1) };
                case GameShapes.Square:
                    return new Vector2I[] { new Vector2I(0, 0), new Vector2I(0, 1), new Vector2I(1, 0), new Vector2I(1, 1) };
                case GameShapes.T:
                    return new Vector2I[] { new Vector2I(0, 0), new Vector2I(1, 0), new Vector2I(-1, 0), new Vector2I(0, 1) };
                case GameShapes.Polygon:
                    return new Vector2I[] { new Vector2I(0, 0) };
                default:
                    return null;
            }
        }

        public static Vector2I[] GetShape(GameShapes shape, Vector2I center)
        {
            Vector2I[] temp = GetShape(shape);

            temp[0] = center;

            return temp;
        }
    }
}
