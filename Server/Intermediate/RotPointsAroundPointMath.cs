using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intermediate
{
    class RotPointsAroundPointMath
    {
        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        private static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        /// <summary>
        /// Rotate the points in pnts around the point origin an angle degreeAngle.
        /// </summary>
        /// <param name="pnts"></param>
        /// <param name="origin"></param>
        /// <param name="degreeAngle"></param>
        public static Vector2I[] RotatePoints(Vector2I[] pnts, Vector2I origin, double degreeAngle)
        {
            Vector2I[] pts = new Vector2I[pnts.Length];
            for (int i = 0; i < pnts.Length; i++)
            {
                pts[i] = RotatePoint(pnts[i], origin, degreeAngle);
            }

            return pts;
        }

        /// <summary>
        /// Rotate a point around a point (origin) an angle (degreeAngle).
        /// </summary>
        /// <param name="pnt"></param>
        /// <param name="origin"></param>
        /// <param name="degreeAngle"></param>
        /// <returns></returns>
        public static Vector2I RotatePoint(Vector2I pnt, Vector2I origin, double degreeAngle)
        {
            double radAngle = DegreeToRadian(degreeAngle);

            double deltaX = pnt.X - origin.X;
            double deltaY = pnt.Y - origin.Y;

            Vector2I newPoint = new Vector2I((int)(origin.X + (Math.Cos(radAngle) * deltaX - Math.Sin(radAngle) * deltaY)),
                                            (int)(origin.Y + (Math.Sin(radAngle) * deltaX + Math.Cos(radAngle) * deltaY)));

            return newPoint;
        }
    }
}
