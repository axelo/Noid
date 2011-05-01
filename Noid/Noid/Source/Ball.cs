using System;
using Microsoft.Xna.Framework;

namespace Noid
{
    class Ball
    {
        public Vector2 Velocity;
        public Circle Circle { get; private set; }
        public Vector2 LastPosition;

        public Ball()
        {
            Circle = new Circle(20, 20, 32);
            LastPosition = Circle.Position;
        }

        public void VelocityFromAngle(float angle, float amplitude)
        {
            Velocity.X = (float)(Math.Cos(angle) * amplitude);
            Velocity.Y = (float)(Math.Sin(angle) * amplitude);
        }
    }
}
