using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Collision
{
    public enum CollisionComponentType
    {
        Vertex,
        Edge,
    }

    public class CollidableObject
    {
        public Vector2 Position { get; set; }
        public float Rotation { get; set; } = 0;
        public Vector2 Scale { get; set; } = Vector2.One;

        public Vector2 Velocity { get; set; } = Vector2.Zero;

        public List<Vector2> CollisionAxes { get; private set; } = new List<Vector2>();

        public bool IsColliding { get; set; } = false;
        public float CollisionTime { get; set; }
        public Vector2 PushVector { get; set; }

        public string Name { get; set; }

        public CollidableObject(Vector2 dimensions)
        {
            localSpaceVertices.Add(new Vector2(-dimensions.X / 2, -dimensions.Y / 2));
            localSpaceVertices.Add(new Vector2(dimensions.X / 2, -dimensions.Y / 2));
            localSpaceVertices.Add(new Vector2(dimensions.X / 2, dimensions.Y / 2));
            localSpaceVertices.Add(new Vector2(-dimensions.X / 2, dimensions.Y / 2));
        }

        public CollidableObject(List<Vector2> vertices)
        {
            localSpaceVertices.AddRange(vertices);
        }

        public void CalculateInterval(Vector2 axis, out float min, out float max)
        {
            min = float.MaxValue;
            max = float.MinValue;

            foreach(var v in worldSpaceVertices)
            {
                var p = Vector2.Dot(axis, v);
                min = Math.Min(min, p);
                max = Math.Max(max, p);
            }
        }

        private static bool FindIntervalIntersection(CollidableObject object1, CollidableObject object2, Vector2 velocity, Vector2 axis, out float time)
        {
            float min0, max0, min1, max1;

            object1.CalculateInterval(axis, out min0, out max0);
            object2.CalculateInterval(axis, out min1, out max1);

            float d0 = min0 - max1;
            float d1 = min1 - max0;

            if (Utility.ApproximatelyZero(d0))
                d0 = 0;
            if (Utility.ApproximatelyZero(d1))
                d1 = 0;

            if (d0 < 0 && d1 < 0)
            {
                time = Math.Max(d0, d1);
                return true;
            }

            float v = Vector2.Dot(velocity, axis);
            if (Utility.ApproximatelyZero(v))
            {
                time = float.NegativeInfinity;
                return false;
            }

            float t0 = -d0 / v;
            float t1 = d1 / v;

            if (t0 <= 0 && t1 <= 0)
            {
                time = float.NegativeInfinity;
                return false;
            }

            if (t0 > t1)
            {
                float temp = t0;
                t0 = t1;
                t1 = temp;
            }

            time = (t0 >= 0) ? t0 : t1;
            if (time >= 1)
            {
                return false;
            }

            return true;
        }

        public static bool FindCollision(CollidableObject object1, CollidableObject object2, out Vector2 axis, out float time)
        {
            Vector2 velocity = object1.Velocity - object2.Velocity;

            axis = default(Vector2);
            time = default(float);

            Vector2 velocityNormal = Vector2.Normalize(new Vector2(velocity.Y, -velocity.X));

            float tempTime;

            // FIXME: if velocity magnitude is zero, we can skip this
            // FIXME: maybe we should test this last, if we're overlapping it's not needed
            if (!FindIntervalIntersection(object1, object2, velocity, velocityNormal, out tempTime))
                return false;

            float maxNegativeTime = float.MinValue;
            float maxPositiveTime = float.MinValue;
            Vector2 maxNegativeAxis = Vector2.Zero;
            Vector2 maxPositiveAxis = Vector2.Zero;
            bool futureCollision = false;

            foreach (var testAxis in object1.CollisionAxes)
            {
                if (!FindIntervalIntersection(object1, object2, velocity, testAxis, out tempTime))
                {
                    return false;
                }

                if (tempTime >= 0)
                {
                    if (tempTime > maxPositiveTime)
                    {
                        maxPositiveTime = tempTime;
                        maxPositiveAxis = testAxis;
                    }

                    futureCollision = true;
                }
                else
                {
                    if (tempTime > maxNegativeTime)
                    {
                        maxNegativeTime = tempTime;
                        maxNegativeAxis = testAxis;
                    }
                }
            }

            foreach (var testAxis in object2.CollisionAxes)
            {
                if (!FindIntervalIntersection(object1, object2, velocity, testAxis, out tempTime))
                {
                    return false;
                }

                if (tempTime >= 0)
                {
                    if (tempTime > maxPositiveTime)
                    {
                        maxPositiveTime = tempTime;
                        maxPositiveAxis = testAxis;
                    }

                    futureCollision = true;
                }
                else
                {
                    if (tempTime > maxNegativeTime)
                    {
                        maxNegativeTime = tempTime;
                        maxNegativeAxis = testAxis;
                    }
                }
            }

            if (futureCollision)
            {
                time = maxPositiveTime;
                axis = maxPositiveAxis;
            }
            else
            {
                time = maxNegativeTime;
                axis = maxNegativeAxis;
            }

            if (Vector2.Dot(object1.Position - object2.Position, axis) < 0.0f)
                axis *= -1.0f;

            return true;
        }

        public CollisionComponentType GetCollisionComponent(Vector2 direction, out int vertex1, out int vertex2)
        {
            vertex1 = vertex2 = default(int);

            float max = float.MinValue;
            CollisionComponentType cct = CollisionComponentType.Vertex;

            for (int i = 0; i < worldSpaceVertices.Count; i++)
            {
                var v = worldSpaceVertices[i];
                var temp = Vector2.Dot(direction, v);

                if (Utility.ApproximatelyEqual(temp, max))
                {
                    vertex2 = i;
                    cct = CollisionComponentType.Edge;
                }
                else if (temp > max)
                {
                    max = temp;
                    vertex1 = i;
                    cct = CollisionComponentType.Vertex;
                }
            }

            return cct;
        }

        public void UpdateWorldSpaceVertices()
        {
            worldSpaceVertices.Clear();
            foreach(var v in localSpaceVertices)
            {
                worldSpaceVertices.Add(TranformPoint(v, Position, Rotation, Scale));
            }

            CollisionAxes.Clear();
            var lastVertex = worldSpaceVertices[worldSpaceVertices.Count - 1];
            foreach(var v in worldSpaceVertices)
            {
                var temp = v - lastVertex;
                temp = new Vector2(temp.Y, -temp.X);
                temp.Normalize();
                CollisionAxes.Add(temp);
                lastVertex = v;
            }
        }

        public void Render(DebugDraw debugDraw)
        {
            debugDraw.DrawShape(worldSpaceVertices, Color.White);

            if (Velocity.Length() > 0.01f)
            {
                if (IsColliding)
                {
                    if (CollisionTime > 0)
                    {
                        debugDraw.DrawShape(worldSpaceVertices, Velocity * CollisionTime, Color.Blue);

                        debugDraw.DrawArrow(Position, Position + Velocity * CollisionTime, Color.Red);

                        var cct = GetCollisionComponent(PushVector, out int vertexIndex1, out int vertexIndex2);
                        debugDraw.DrawPoint(worldSpaceVertices[vertexIndex1], Color.White, 10);
                        debugDraw.DrawPoint(worldSpaceVertices[vertexIndex1] + Velocity * CollisionTime, Color.Blue, 10);
                        if (cct == CollisionComponentType.Edge)
                        {
                            debugDraw.DrawPoint(worldSpaceVertices[vertexIndex2], Color.White, 10);
                            debugDraw.DrawPoint(worldSpaceVertices[vertexIndex2] + Velocity * CollisionTime, Color.Blue, 10);
                        }
                    }
                    else if(CollisionTime < 0)
                    {
                        debugDraw.DrawShape(worldSpaceVertices, PushVector, Color.Yellow);

                        debugDraw.DrawArrow(Position, Position + PushVector, Color.Yellow);
                    }
                }
                else
                {
                    debugDraw.DrawShape(worldSpaceVertices, Velocity, Color.Blue);

                    debugDraw.DrawArrow(Position, Position + Velocity, Color.Green);
                }
            }
        }

        public static Vector2 TranformPoint(Vector2 point, Vector2 translation, float rotation, Vector2 scale)
        {
            float cos = (float)Math.Cos(rotation);
            float sin = (float)Math.Sin(rotation);

            // scale, rotate, then translate
            Vector2 scaled = scale * point;
            Vector2 rotated = new Vector2(scaled.X * cos - scaled.Y * sin, scaled.X * sin + scaled.Y * cos);
            Vector2 result = rotated + translation;

            return result;
        }

        List<Vector2> localSpaceVertices = new List<Vector2>();
        List<Vector2> worldSpaceVertices = new List<Vector2>();
    }
}