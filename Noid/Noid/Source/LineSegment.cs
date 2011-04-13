using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Noid
{
    class LineSegment
    {
        private Vector2 _A;
        private Vector2 _B;

        public Vector2 A { get { return _A; } }
        public Vector2 B { get { return _B; } }

        public LineSegment(float x1, float y1, float x2, float y2)
        {
            _A.X = x1;
            _A.Y = y1;
            _B.X = x2;
            _B.Y = y2;
        }
    }
}
