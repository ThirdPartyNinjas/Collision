using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Collision
{
    public class CollisionGame : Game
    {
        DebugDraw debugDraw;
        CollidableObject box1;

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
                Position = new Vector2(1280, 720) / 2,
            };
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            box1.Rotation += (float)gameTime.ElapsedGameTime.TotalSeconds;
            box1.Scale = new Vector2(1.0f, 1.0f + (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds));
            box1.UpdateWorldSpaceVertices();
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            GraphicsDevice.Clear(Color.CornflowerBlue);

            box1.Render(debugDraw, Color.White);
        }

        //private static bool FindIntervalIntersection(CollidableObject object1, CollidableObject object2, Vector2 velocity, Vector2 axis, out float time)
        //{
        //    float min0, max0, min1, max1;

        //    object1.CalculateInterval(axis, out min0, out max0);
        //    object2.CalculateInterval(axis, out min1, out max1);

        //    float d0 = min0 - max1;
        //    float d1 = min1 - max0;

        //    if (Mathf.Approximately(d0, 0))
        //        d0 = 0;
        //    if (Mathf.Approximately(d1, 0))
        //        d1 = 0;

        //    if (d0 < 0 && d1 < 0)
        //    {
        //        time = Mathf.Max(d0, d1);
        //        return true;
        //    }

        //    float v = Vector2.Dot(velocity, axis);
        //    if (Mathf.Approximately(v, 0))
        //    {
        //        time = float.NegativeInfinity;
        //        return false;
        //    }

        //    float t0 = -d0 / v;
        //    float t1 = d1 / v;

        //    if (t0 <= 0 && t1 <= 0)
        //    {
        //        time = float.NegativeInfinity;
        //        return false;
        //    }

        //    if (t0 > t1)
        //    {
        //        float temp = t0;
        //        t0 = t1;
        //        t1 = temp;
        //    }

        //    time = (t0 >= 0) ? t0 : t1;
        //    if (time >= 1)
        //    {
        //        return false;
        //    }

        //    return true;
        //}

        //public bool FindCollision(CollidableObject otherObject, Vector2 velocity, out Vector2 axis, out float time)
        //{
        //    UpdateWorldSpacePoints();
        //    otherObject.UpdateWorldSpacePoints();

        //    axis = default(Vector2);
        //    time = default(float);

        //    if (collisionAxes == null)
        //        collisionAxes = new Vector2[4];

        //    Vector2 velocityNormal = (new Vector2(velocity.y, -velocity.x)).normalized;

        //    float tempTime;

        //    // FIXME: if velocity magnitude is zero, we can skip this
        //    // FIXME: maybe we should test this last, if we're overlapping it's not needed
        //    if (!FindIntervalIntersection(this, otherObject, velocity, velocityNormal, out tempTime))
        //        return false;

        //    collisionAxes[0] = UpAxis;
        //    collisionAxes[1] = RightAxis;
        //    collisionAxes[2] = otherObject.UpAxis;
        //    collisionAxes[3] = otherObject.RightAxis;

        //    float maxNegativeTime = float.MinValue;
        //    float maxPositiveTime = float.MinValue;
        //    Vector2 maxNegativeAxis = Vector2.zero;
        //    Vector2 maxPositiveAxis = Vector2.zero;
        //    bool futureCollision = false;

        //    foreach (var testAxis in collisionAxes)
        //    {
        //        if (!FindIntervalIntersection(this, otherObject, velocity, testAxis, out tempTime))
        //        {
        //            return false;
        //        }

        //        if (tempTime >= 0)
        //        {
        //            if (tempTime > maxPositiveTime)
        //            {
        //                maxPositiveTime = tempTime;
        //                maxPositiveAxis = testAxis;
        //            }

        //            futureCollision = true;
        //        }
        //        else
        //        {
        //            if (tempTime > maxNegativeTime)
        //            {
        //                maxNegativeTime = tempTime;
        //                maxNegativeAxis = testAxis;
        //            }
        //        }
        //    }

        //    if (futureCollision)
        //    {
        //        time = maxPositiveTime;
        //        axis = maxPositiveAxis;
        //    }
        //    else
        //    {
        //        time = maxNegativeTime;
        //        axis = maxNegativeAxis;
        //    }

        //    if (time < 0.0f && Vector2.Dot(transform.position - otherObject.transform.position, axis) < 0.0f)
        //        axis *= -1.0f;

        //    return true;
        //}

        //public CollisionComponentType GetCollisionComponent(Vector2 direction, out int vertex1, out int vertex2)
        //{
        //    vertex1 = vertex2 = default(int);

        //    float max = float.MinValue;
        //    CollisionComponentType cct = CollisionComponentType.Vertex;

        //    for (int i = 0; i < 4; i++)
        //    {
        //        var v = worldSpacePoints[i];
        //        var temp = Vector2.Dot(direction, v);

        //        if (temp > max)
        //        {
        //            max = temp;
        //            vertex1 = i;
        //            cct = CollisionComponentType.Vertex;
        //        }
        //        else if (Mathf.Approximately(temp, max))
        //        {
        //            vertex2 = i;
        //            cct = CollisionComponentType.Edge;
        //        }
        //    }

        //    return cct;
        //}

    }
}