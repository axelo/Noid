using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Noid
{
    class Collision
    {
        static public ICollection<Vector2> CollidedSurfaceNormals(Circle circle, ICollection<AABB> aabbs)
        {
            var surfaceNormals = new List<Vector2>();

        restartLoop:
            foreach (var aabb in aabbs)
            {
                Vector2 nearestVector = NearestVectorBetweenCircleAndAABB(aabb, circle);

                if (NearestVectorIntersects(nearestVector, circle))
                {
                    var surfaceNormal = Vector2.Normalize(nearestVector);

                    if (!surfaceNormals.Contains(surfaceNormal))
                    {
                        surfaceNormals.Add(surfaceNormal);

                        PushCircleApartFromAABB(circle, nearestVector, 1.0f);

                        goto restartLoop;
                    }
                }
            }

            return surfaceNormals;
        }

        static public Vector2 AverageSurfaceNormal(ICollection<Vector2> normals)
        {
            Vector2 surfaceNormal = new Vector2(0, 0);

            foreach (var normal in normals)
            {
                surfaceNormal += normal;
            }

            surfaceNormal /= normals.Count;

            surfaceNormal.Normalize();

            return surfaceNormal;
        }

        static public bool NearestVectorIntersects(Vector2 nearestVector, Circle circle)
        {
            return (nearestVector.Length() - circle.Radius) < 0;
        }

        static public Vector2 NearestVectorBetweenCircleAndAABB(AABB aabb, Circle circle)
        {
            Vector2 v = circle.Position - aabb.Position;

            Vector2 vClamped = Vector2.Clamp(v, -aabb.HalfExtent, aabb.HalfExtent);

            return v - vClamped;
        }

        static public void PushCircleApartFromAABB(Circle circle, Vector2 nearestVector, float extra)
        {
            float distance = nearestVector.Length() - (circle.Radius + extra);

            Vector2 surfaceNormal = Vector2.Normalize(nearestVector);
            circle.Position -= surfaceNormal * distance;
        }
    }
}
