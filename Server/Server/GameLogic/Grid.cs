using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Grid
    {
        private int width;
        private int height;
        private List<GameObject> objects;

        public int Width
        {
            get
            {
                return width;
            }
            set
            {
                width = value;
            }
        }
        public int Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
            }
        }

        public List<GameObject> Objects
        {
            get { return objects; }
        }

        public Grid(int width, int height)
        {

        }
    }
}
