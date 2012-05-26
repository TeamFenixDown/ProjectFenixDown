using System;
using Microsoft.Xna.Framework;

namespace ProjectFenixDown
{
    public static class RectangleExtensions
    {
        //calculates the signed deapth of interesction between two rectangles
        //This allows callers to determine the correct direction to push objects in order to resolve collisions.
        //if the rectangles are not intersecting, then Vector2.Zero is returned.
        public static Vector2 GetIntersectionDepth(this Rectangle rectA, Rectangle rectB)
        {
            //calculate half sizes
            float halfWidthA = rectA.Width / 2.0f;
            float halfHeightA = rectA.Height / 2.0f;
            float halfWidthB = rectB.Width / 2.0f;
            float halfHeightB = rectB.Height / 2.0f;

            //calculate centers
            Vector2 centerA = new Vector2(rectA.Left + halfWidthA, rectA.Top + halfHeightA);
            Vector2 centerB = new Vector2(rectB.Left + halfWidthB, rectB.Top + halfHeightB);

            //calculate the current and minimum-non-intersecting distances between centers
            float distanceX = centerA.X - centerB.X;
            float distanceY = centerA.Y - centerB.Y;
            float minDistanceX = halfWidthA + halfWidthB;
            float minDistanceY = halfHeightA + halfHeightB;

            //if we are not intersecting at all, return (0,0)
            if (Math.Abs(distanceX) >= minDistanceX || Math.Abs(distanceY) >= minDistanceY)
            {
                return Vector2.Zero;
            }

            //calculate and return intersecting depths
            float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
            float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;
            return new Vector2(depthX, depthY);
        }

        //gets the position of the center of the bottom edge of the rectangle
        public static Vector2 getBottomCenter(this Rectangle rect)
        {
            return new Vector2(rect.X + rect.Width / 2.0f, rect.Bottom);
        }
    }
}
