using Microsoft.Xna.Framework;
using System;

namespace Collision
{
    public class CollidableObject
    {
        public Vector2 Position { get; set; }
        public float Rotation { get; set; } = 0;
        public Vector2 Scale { get; set; } = Vector2.One;

        public Vector2 Dimensions { get; set; }
        public Vector2 Velocity { get; set; } = Vector2.Zero;

        public void CalculateInterval(Vector2 axis, out float min, out float max)
        {
            float a = Vector2.Dot(axis, worldSpaceVertices[0]);
            float b = Vector2.Dot(axis, worldSpaceVertices[1]);
            float c = Vector2.Dot(axis, worldSpaceVertices[2]);
            float d = Vector2.Dot(axis, worldSpaceVertices[3]);

            min = Math.Min(Math.Min(Math.Min(a, b), c), d);
            max = Math.Max(Math.Max(Math.Max(a, b), c), d);
        }

        public void UpdateWorldSpaceVertices()
        {
            worldSpaceVertices[0] = TranformPoint(new Vector2(-Dimensions.X / 2, -Dimensions.Y / 2), Position, Rotation, Scale);
            worldSpaceVertices[1] = TranformPoint(new Vector2(Dimensions.X / 2, -Dimensions.Y / 2), Position, Rotation, Scale);
            worldSpaceVertices[2] = TranformPoint(new Vector2(Dimensions.X / 2, Dimensions.Y / 2), Position, Rotation, Scale);
            worldSpaceVertices[3] = TranformPoint(new Vector2(-Dimensions.X / 2, Dimensions.Y / 2), Position, Rotation, Scale);
        }

        public void Render(DebugDraw debugDraw, Color color)
        {
            debugDraw.DrawLine(worldSpaceVertices[0], worldSpaceVertices[1], color);
            debugDraw.DrawLine(worldSpaceVertices[1], worldSpaceVertices[2], color);
            debugDraw.DrawLine(worldSpaceVertices[2], worldSpaceVertices[3], color);
            debugDraw.DrawLine(worldSpaceVertices[3], worldSpaceVertices[0], color);
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