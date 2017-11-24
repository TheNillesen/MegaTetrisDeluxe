using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Intermediate
{
    [Serializable]
    public partial class Vector2I
    {
        public static Vector2I operator + (Vector2I lhs, Vector2I rhs)
        {
            return new Vector2I(lhs.X + rhs.X, lhs.Y + rhs.Y);
        }
        public static Vector2I operator - (Vector2I lhs, Vector2I rhs)
        {
            return new Vector2I(lhs.X - rhs.X, lhs.Y - rhs.Y);
        }
        public static Vector2I operator * (Vector2I lhs, int rhs)
        {
            return new Vector2I(lhs.X * rhs, lhs.Y * rhs);
        }
        public static Vector2I operator * (Vector2I lhs, float rhs)
        {
            return new Vector2I((int)(lhs.X * rhs), (int)(lhs.Y * rhs));
        }
        public static Vector2I operator / (Vector2I lhs, int rhs)
        {
            return new Vector2I(lhs.X / rhs, lhs.Y / rhs);
        }
        public static Vector2I operator / (Vector2I lhs, float rhs)
        {
            return new Vector2I((int)(lhs.X / rhs), (int)(lhs.Y / rhs));
        }

        public int X { get; set; }
        public int Y { get; set; }
        public float Length { get { return (float)Math.Sqrt(X * X + Y * Y); } }

        public Vector2I(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public static byte[] Serialize(Vector2I vector)
        {
            return vector.Serialize();
        }

        public byte[] Serialize()
        {
            BinaryFormatter ser = new BinaryFormatter();
            byte[] result;

            using (MemoryStream mem = new MemoryStream())
            {
                ser.Serialize(mem, this);

                result = mem.ToArray();
            }

            return result;
        }

        public static Vector2I Deserialize(byte[] VectorSer)
        {
            BinaryFormatter des = new BinaryFormatter();
            Vector2I result;

            using (MemoryStream mem = new MemoryStream(VectorSer))
            {
                result = des.Deserialize(mem) as Vector2I;
            }

            return result;
        }
    }
}
