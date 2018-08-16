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
                Velocity = new Vector2(-20, 100),
            };
            box2 = new CollidableObject()
            {
                Dimensions = new Vector2(500, 25),
                Position = new Vector2(640, 500),
                Velocity = new Vector2(20, -50),
            };
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Vector2 newPosition = Vector2.Zero;
            Vector2 newVelocity = Vector2.Zero;

            var keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Left))
                newPosition.X = -1;
            else if (keyboardState.IsKeyDown(Keys.Right))
                newPosition.X = 1;
            if (keyboardState.IsKeyDown(Keys.Up))
                newPosition.Y = -1;
            else if (keyboardState.IsKeyDown(Keys.Down))
                newPosition.Y = 1;

            if (keyboardState.IsKeyDown(Keys.A))
                newVelocity.X = -1;
            else if (keyboardState.IsKeyDown(Keys.D))
                newVelocity.X = 1;
            if (keyboardState.IsKeyDown(Keys.W))
                newVelocity.Y = -1;
            else if (keyboardState.IsKeyDown(Keys.S))
                newVelocity.Y = 1;

            box1.Velocity += newVelocity;
            box1.Position += newPosition;
            box1.Rotation += (float)gameTime.ElapsedGameTime.TotalSeconds;

            box2.Rotation = (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds) / 4.0f;

            box1.UpdateWorldSpaceVertices();
            box2.UpdateWorldSpaceVertices();

            if(CollidableObject.FindCollision(box1, box2, out Vector2 axis, out float time))
            {
                box1.IsColliding = true;
                box1.CollisionTime = time;
                box1.PushVector = axis * -time;

                box2.IsColliding = true;
                box2.CollisionTime = Math.Max(time, 0);
                box2.PushVector = axis;
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