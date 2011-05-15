using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Noid
{
    class AABB
    {
        public Vector2 Position;
        public Vector2 HalfExtent;

        public Color Color;

        public AABB(float x, float y, float width, float height)
        {
            HalfExtent.X = width / 2.0f;
            HalfExtent.Y = height / 2.0f;
            Position.X = x + HalfExtent.X;
            Position.Y = y + HalfExtent.Y;
        }
    }
}
