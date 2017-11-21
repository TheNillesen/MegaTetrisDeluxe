using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class NetworkController : Component
    {
        private Guid id;

        public Guid ID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }

        public NetworkController(GameObject gameObject, Guid id) : base(gameObject)
        {
            this.id = id;
        }
    }
}
