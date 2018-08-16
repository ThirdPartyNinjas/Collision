using System;

namespace Collision
{
    public class Utility
    {
        public static bool ApproximatelyEqual(float f, float value, float tolerance = 0.0001f)
        {
            // http://realtimecollisiondetection.net/blog/?p=89
            if (Math.Abs(f - value) <= tolerance * Math.Max(1.0f, Math.Max(Math.Abs(f), Math.Abs(value))))
            {
                return true;
            }
            return false;
        }

        public static bool ApproximatelyZero(float f, float tolerance = 0.0001f)
        {
            if (Math.Abs(f) < tolerance)
            {
                return true;
            }
            return false;
        }
    }
}