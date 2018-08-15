using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Collision
{
    public class DebugDraw
    {
        public DebugDraw(GraphicsDevice graphicsDevice)
        {
            spriteBatch = new SpriteBatch(graphicsDevice);

            texture = new Texture2D(graphicsDevice, 1, 1);
            texture.SetData<Color>(new Color[] { Color.White });
        }

        public void DrawLine(Vector2 start, Vector2 end, Color color, float thickness = 1.0f)
        {
            Vector2 difference = end - start;
            float length = difference.Length();
            difference.Normalize();

            float angle = (float)Math.Atan2(difference.Y, difference.X);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);

            spriteBatch.Draw(texture, start, null, color, angle, new Vector2(0, 0.5f), new Vector2(length, thickness), SpriteEffects.None, 0);

            spriteBatch.End();
        }

        public void DrawLineList(IList<Vector2> points, Color color, float thickness = 1.0f)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);

            for (int i = 0; i < points.Count - 1; i++)
            {
                Vector2 start = points[i];
                Vector2 end = points[i + 1];

                Vector2 difference = end - start;
                float length = difference.Length();
                difference.Normalize();

                float angle = (float)Math.Atan2(difference.Y, difference.X);

                spriteBatch.Draw(texture, start, null, color, angle, new Vector2(0, 0.5f), new Vector2(length, thickness), SpriteEffects.None, 0);
            }
            spriteBatch.End();
        }

        public void DrawArrow(Vector2 start, Vector2 end, Color color, float thickness = 1.0f, float arrowHeadLength = 15.0f, float arrowHeadAngle = (25.0f * MathHelper.Pi / 180.0f))
        {
            Vector2 difference = end - start;
            float length = difference.Length();
            difference.Normalize();

            float angle = (float)Math.Atan2(difference.Y, difference.X);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);

            spriteBatch.Draw(texture, start, null, color, angle, new Vector2(0, 0.5f), new Vector2(length, thickness), SpriteEffects.None, 0);
            spriteBatch.Draw(texture, end, null, color, angle + (MathHelper.Pi - arrowHeadAngle), new Vector2(0, 0.5f), new Vector2(arrowHeadLength, thickness), SpriteEffects.None, 0);
            spriteBatch.Draw(texture, end, null, color, angle + (MathHelper.Pi + arrowHeadAngle), new Vector2(0, 0.5f), new Vector2(arrowHeadLength, thickness), SpriteEffects.None, 0);

            spriteBatch.End();
        }

        public void DrawRectangle(Vector2 position, Vector2 scale, float rotation, Color color)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            spriteBatch.Draw(texture, position, null, color, rotation, new Vector2(0.5f, 0.5f), scale, SpriteEffects.None, 0);
            spriteBatch.End();
        }

        private SpriteBatch spriteBatch;
        private Texture2D texture;
    }
}