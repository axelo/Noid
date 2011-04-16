using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Noid
{
    class Brick
    {
        public Vector2 Position;
        public Vector2 Size;

        public ICollection<LineSegment> Walls { get; private set; }

        public Brick(float x, float y, float width, float height)
        {
            Position.X = x;
            Position.Y = y;
            Size.X = width;
            Size.Y = height;
            Walls = createWalls();
        }

        private ICollection<LineSegment> createWalls()
        {
            var walls = new List<LineSegment>();

            walls.Add(new LineSegment(Position.X, Position.Y, Position.X + Size.X, Position.Y));
            walls.Add(new LineSegment(Position.X + Size.X, Position.Y, Position.X + Size.X, Position.Y + Size.Y));
            walls.Add(new LineSegment(Position.X, Position.Y + Size.Y, Position.X + Size.X, Position.Y + Size.Y));
            walls.Add(new LineSegment(Position.X, Position.Y, Position.X, Position.Y + Size.Y));

            return walls;
        }
    }
}
