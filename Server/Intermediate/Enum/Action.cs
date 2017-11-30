using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intermediate
{
    public enum Action : byte
    {
        Left = 0x00,
        Rigth = 0x01,
        Down = 0x02,
        Instant = 0x03,
        RotateLeft = 0x04,
        RotateRight = 0x05
    }
}
