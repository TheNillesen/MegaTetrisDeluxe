using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Component
    {
        public GameObject gameObject;
        public Spriterendere Spriterendere { get { return gameObject.Rendere; } }
        public Transform Transform { get { return gameObject.Transform; } }

        public Component(GameObject gameObject)
        {
            this.gameObject = gameObject;
        }
    }
}
