using Microsoft.Xna.Framework;
using System;

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

        public Vector2 Dimensions { get; set; }
        public Vector2 Velocity { get; set; } = Vector2.Zero;

        public Vector2[] CollisionAxes { get; private set; } = new Vector2[2];

        public bool IsColliding { get; set; } = false;
        public float CollisionTime { get; set; }
        public Vector2 PushVector { get; set; }

        public void CalculateInterval(Vector2 axis, out float min, out float max)
        {
            float a = Vector2.Dot(axis, worldSpaceVertices[0]);
            float b = Vector2.Dot(axis, worldSpaceVertices[1]);
            float c = Vector2.Dot(axis, worldSpaceVertices[2]);
            float d = Vector2.Dot(axis, worldSpaceVertices[3]);

            min = Math.Min(Math.Min(Math.Min(a, b), c), d);
            max = Math.Max(Math.Max(Math.Max(a, b), c), d);
        }

        private static bool FindIntervalIntersection(CollidableObject object1, CollidableObject object2, Vector2 velocity, Vector2 axis, out float time)
        {
            float min0, max0, min1, max1;

            object1.CalculateInterval(axis, out min0, out max0);
            object2.CalculateInterval(axis, out min1, out max1);

            float d0 = min0 - max1;
            float d1 = min1 - max0;

            if (Math.Abs(d0) < 0.0001f)
                d0 = 0;
            if (Math.Abs(d1) < 0.0001f)
                d1 = 0;

            if (d0 < 0 && d1 < 0)
            {
                time = Math.Max(d0, d1);
                return true;
            }

            float v = Vector2.Dot(velocity, axis);
            if (Math.Abs(v) < 0.0001f)
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

            Vector2[] collisionAxes = new Vector2[4];
            collisionAxes[0] = object1.CollisionAxes[0];
            collisionAxes[1] = object1.CollisionAxes[1];
            collisionAxes[2] = object2.CollisionAxes[0];
            collisionAxes[3] = object2.CollisionAxes[1];

            float maxNegativeTime = float.MinValue;
            float maxPositiveTime = float.MinValue;
            Vector2 maxNegativeAxis = Vector2.Zero;
            Vector2 maxPositiveAxis = Vector2.Zero;
            bool futureCollision = false;

            foreach (var testAxis in collisionAxes)
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

            if (time < 0.0f && Vector2.Dot(object1.Position - object2.Position, axis) < 0.0f)
                axis *= -1.0f;

            return true;
        }

        public CollisionComponentType GetCollisionComponent(Vector2 direction, out int vertex1, out int vertex2)
        {
            vertex1 = vertex2 = default(int);

            float max = float.MinValue;
            CollisionComponentType cct = CollisionComponentType.Vertex;

            for (int i = 0; i < 4; i++)
            {
                var v = worldSpaceVertices[i];
                var temp = Vector2.Dot(direction, v);

                if (temp > max)
                {
                    max = temp;
                    vertex1 = i;
                    cct = CollisionComponentType.Vertex;
                }
                else if (Math.Abs(temp - max) < 0.0001f)
                {
                    vertex2 = i;
                    cct = CollisionComponentType.Edge;
                }
            }

            return cct;
        }

        public void UpdateWorldSpaceVertices()
        {
            worldSpaceVertices[0] = TranformPoint(new Vector2(-Dimensions.X / 2, -Dimensions.Y / 2), Position, Rotation, Scale);
            worldSpaceVertices[1] = TranformPoint(new Vector2(Dimensions.X / 2, -Dimensions.Y / 2), Position, Rotation, Scale);
            worldSpaceVertices[2] = TranformPoint(new Vector2(Dimensions.X / 2, Dimensions.Y / 2), Position, Rotation, Scale);
            worldSpaceVertices[3] = TranformPoint(new Vector2(-Dimensions.X / 2, Dimensions.Y / 2), Position, Rotation, Scale);

            var temp = worldSpaceVertices[1] - worldSpaceVertices[0];
            CollisionAxes[0] = new Vector2(temp.Y, -temp.X);
            CollisionAxes[0].Normalize();
            temp = worldSpaceVertices[2] - worldSpaceVertices[1];
            CollisionAxes[1] = new Vector2(temp.Y, -temp.X);
            CollisionAxes[1].Normalize();
        }

        public void Render(DebugDraw debugDraw)
        {
            debugDraw.DrawLine(worldSpaceVertices[0], worldSpaceVertices[1], Color.White);
            debugDraw.DrawLine(worldSpaceVertices[1], worldSpaceVertices[2], Color.White);
            debugDraw.DrawLine(worldSpaceVertices[2], worldSpaceVertices[3], Color.White);
            debugDraw.DrawLine(worldSpaceVertices[3], worldSpaceVertices[0], Color.White);

            if (Velocity.Length() > 0.01f)
            {
                if (IsColliding)
                {
                    if(CollisionTime > 0)
                    {
                        debugDraw.DrawLine(worldSpaceVertices[0] + Velocity * CollisionTime, worldSpaceVertices[1] + Velocity * CollisionTime, Color.Blue);
                        debugDraw.DrawLine(worldSpaceVertices[1] + Velocity * CollisionTime, worldSpaceVertices[2] + Velocity * CollisionTime, Color.Blue);
                        debugDraw.DrawLine(worldSpaceVertices[2] + Velocity * CollisionTime, worldSpaceVertices[3] + Velocity * CollisionTime, Color.Blue);
                        debugDraw.DrawLine(worldSpaceVertices[3] + Velocity * CollisionTime, worldSpaceVertices[0] + Velocity * CollisionTime, Color.Blue);

                        debugDraw.DrawArrow(Position, Position + Velocity * CollisionTime, Color.Red);
                    }
                }
                else
                {
                    debugDraw.DrawLine(worldSpaceVertices[0] + Velocity, worldSpaceVertices[1] + Velocity, Color.Blue);
                    debugDraw.DrawLine(worldSpaceVertices[1] + Velocity, worldSpaceVertices[2] + Velocity, Color.Blue);
                    debugDraw.DrawLine(worldSpaceVertices[2] + Velocity, worldSpaceVertices[3] + Velocity, Color.Blue);
                    debugDraw.DrawLine(worldSpaceVertices[3] + Velocity, worldSpaceVertices[0] + Velocity, Color.Blue);

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

        Vector2[] worldSpaceVertices = new Vector2[4];
    }
}