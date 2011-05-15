using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Noid
{
    class Collision
    {
        static readonly ICollection<CollisionData> EMPTY = new List<CollisionData>();
        static readonly Vector2 NULL_VECTOR = new Vector2(0, 0);

        static public ICollection<CollisionData> CollisionVectors(Vector2 lastPosition, Circle circle, ICollection<AABB> aabbs)
        {
            float step = 2f; // TODO: Smallest possible AABB-side...
            var dirVector = Vector2.Normalize(circle.Position - lastPosition) * step;

            float movedDistance = Vector2.Distance(lastPosition, circle.Position);

            Circle testCircle = new Circle(lastPosition.X, lastPosition.Y, circle.Radius);

            for (var samples = Math.Floor(movedDistance / step) ; samples >= 0; --samples)
            {
                var normals = CollisionVectors(testCircle, aabbs);

                if (normals.Count > 0)
                {
                    circle.Position = testCircle.Position; // TODO: ugly side effect here :P
                    return normals;
                }

                testCircle.Position += dirVector;
            }

            return EMPTY;
        }

        static public Vector2 AverageVector(IEnumerable<Vector2> vectors)
        {

            if (vectors.Count() < 1)
            {
                return NULL_VECTOR;
            }

            Vector2 average = new Vector2(0, 0);

            foreach (var v in vectors)
            {
                average += v;
            }

            average /= vectors.Count();

            return average;
        }

        static public ICollection<CollisionData> CollisionVectors(Circle circle, IEnumerable<AABB> aabbs)
        {
            var collisionDatas = EMPTY;

            foreach (var aabb in aabbs)
            {
                var collisioVector = CollisionVector(circle, aabb);

                if (collisioVector != NULL_VECTOR)
                {
                    if (collisionDatas == EMPTY) collisionDatas = new List<CollisionData>();

                    var d = new CollisionData { NearestVector = collisioVector, CollidedWith = aabb };

                    collisionDatas.Add(d);
                }
            }

            return collisionDatas;
        }

        static public Vector2 CollisionVector(Circle circle, AABB aabb)
        {
            Vector2 nearestVector = NearestVectorBetweenCircleAndAABB(aabb, circle);

            if (NearestVectorIntersects(nearestVector, circle))
            {
                return nearestVector;
            }

            return NULL_VECTOR;
        }

        static public ICollection<Vector2> CollidedSurfaceNormals(Circle circle, ICollection<AABB> aabbs)
        {
            var surfaceNormals = new List<Vector2>();

        restartLoop:
            foreach (var aabb in aabbs)
            {
                Vector2 nearestVector = NearestVectorBetweenCircleAndAABB(aabb, circle);

                if (NearestVectorIntersects(nearestVector, circle))
                {
                    //if (surfaceNormals == EMPTY) surfaceNormals = new List<Vector2>();

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

        static public Vector2 AverageSurfaceNormal(IEnumerable<Vector2> vectors)
        {
            Vector2 surfaceNormal = new Vector2(0, 0);

            foreach (var v in vectors)
            {
                surfaceNormal += Vector2.Normalize(v);
            }

            surfaceNormal /= vectors.Count();

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
