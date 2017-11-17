using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intermediate.Game
{
    [Serializable]
    public class GameObjectContainer
    {
        private Vector2I position;
        private GameShapes shape;
        private string guid;

        public Vector2I Postion
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        public GameShapes Shape
        {
            get
            {
                return shape;
            }
            set
            {
                shape = value;
            }
        }

        public string Guid
        {
            get
            {
                return guid;
            }
            set
            {
                guid = value;
            }
        }

        public GameObjectContainer(Vector2I position, GameShapes shape, string guid)
        {
            this.position = position;
            this.shape = shape;
            this.guid = guid;
        }
    }
}
