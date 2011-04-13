using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Noid
{
    class Ball
    {
        public Vector2 Velocity;
        public Vector2 Position;
        public float Radius { get { return 64 / 2; } }

        public void VelocityFromAngle(float angle, float amplitude)
        {
            Velocity.X = (float)(Math.Cos(angle) * amplitude);
            Velocity.Y = (float)(Math.Sin(angle) * amplitude);
        }
    }
}
