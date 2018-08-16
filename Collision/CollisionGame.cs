using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Collision
{
    public class CollisionGame : Game
    {
        DebugDraw debugDraw;
        CollidableObject box1, box2;

        public CollisionGame()
        {
            IsFixedTimeStep = false;
            IsMouseVisible = true;

            var graphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 720,
            };
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            debugDraw = new DebugDraw(GraphicsDevice);
            box1 = new CollidableObject()
            {
                Dimensions = new Vector2(50, 50),
                Position = new Vector2(640, 360),
                Velocity = new Vector2(0, 100),
            };
            box2 = new CollidableObject()
            {
                Dimensions = new Vector2(500, 25),
                Position = new Vector2(640, 500),
                Velocity = new Vector2(0, -50),
            };
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Vector2 offset = Vector2.Zero;

            var keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Left))
                offset.X = -1;
            else if (keyboardState.IsKeyDown(Keys.Right))
                offset.X = 1;
            if (keyboardState.IsKeyDown(Keys.Up))
                offset.Y = -1;
            else if (keyboardState.IsKeyDown(Keys.Down))
                offset.Y = 1;

            box1.Position += offset;
            box1.Rotation += (float)gameTime.ElapsedGameTime.TotalSeconds / 10.0f;

            box1.UpdateWorldSpaceVertices();
            box2.UpdateWorldSpaceVertices();

            if(CollidableObject.FindCollision(box1, box2, out Vector2 axis, out float time))
            {
                box1.IsColliding = true;
                box1.CollisionTime = time;
                box1.PushVector = axis * time;

                box2.IsColliding = true;
                box2.CollisionTime = time;
                box2.PushVector = Vector2.Zero;
            }
            else
            {
                box1.IsColliding = box2.IsColliding = false;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            GraphicsDevice.Clear(Color.CornflowerBlue);

            box1.Render(debugDraw);
            box2.Render(debugDraw);
        }

    }
}