﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intermediate;
using Intermediate.Game;

namespace Server
{
    class GameObject
    {
        private Vector2I[] position;
        private GameShapes shape;
        private string guid;

        public Vector2I[] Position
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

        public void Translate(Vector2I direction)
        {
            position[0] += direction;
        }

        public bool ValidateMove(Vector2I direction)
        {
            return true;
        }

        public GameObjectContainer ToGameObjectContainer()
        {
            return new GameObjectContainer(position, shape, guid);
        }
    }
}
