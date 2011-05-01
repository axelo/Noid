using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Noid
{
    class Circle
    {
        public Vector2 Position;
        public float Radius { get; private set; }

        public Circle(Circle copy)
        {
            Position = copy.Position;
            Radius = copy.Radius;
        }

        public Circle(float x, float y, float radius)
        {
            Position.X = x;
            Position.Y = y;
            Radius = radius;
        }
    }
}
