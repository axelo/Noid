using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Noid
{
    class Collision
    {

        public static bool Intersects(LineSegment line, Ball ball)
        {
            float distance = DistanceLineSegmentToPoint(line, ball.Position);

            return distance < ball.Radius;
        }

        public static float DistanceLineSegmentToPoint(LineSegment line, Vector2 p)
        {
            Vector2 nearestPoint = NearestIntersectionPointToLineSegment(line, p);
            return Vector2.Distance(p, nearestPoint);
        }

        public static Vector2 NearestIntersectionPointToLineSegment(LineSegment line, Vector2 p)
        {
            Vector2 V = (line.B - line.A);

            V.Normalize();

            float distanceAlongLine = Vector2.Dot(p, V) - Vector2.Dot(line.A, V);

            if (distanceAlongLine < 0)
            {
                return line.A;
            }
            else if (distanceAlongLine > Vector2.Distance(line.A, line.B))
            {
                return line.B;
            }
            
            return line.A + (distanceAlongLine * V);
        }

        public static Vector2 DistanceToNotCollide(Ball ball, Vector2 collisionPoint)
        {
            Vector2 delta = (collisionPoint - ball.Position);

            float collisionPointToBallCenterAngle = (float) Math.Atan2(delta.Y, delta.X);

            Vector2 perimeterPoint;

            perimeterPoint.X = (float) Math.Cos(collisionPointToBallCenterAngle) * (ball.Radius + 2);
            perimeterPoint.Y = (float) Math.Sin(collisionPointToBallCenterAngle) * (ball.Radius + 2);

            return delta - perimeterPoint;
        }
    }
}
