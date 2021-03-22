using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace GameArchitectureExample.Collisions
{
    /// <summary>
    /// A bounding rectangle for collision detection
    /// </summary>
    public struct BoundingRectangle
    {
        /// <summary>
        /// X coordinate of rectangle origin
        /// </summary>
        public float X;

        /// <summary>
        /// Y coordinate of rectangle origin
        /// </summary>
        public float Y;

        /// <summary>
        /// Width of rectangle
        /// </summary>
        public float Width;

        /// <summary>
        /// Height of rectangle
        /// </summary>
        public float Height;

        /// <summary>
        /// X coordinate of rectangle's left side
        /// </summary>
        public float Left => X;

        /// <summary>
        /// X coordinate of rectangle's right side
        /// </summary>
        public float Right => X + Width;

        /// <summary>
        /// Y coordinate of rectangle's top side
        /// </summary>
        public float Top => Y;

        /// <summary>
        /// Y coordinate of rectangle's bottom side
        /// </summary>
        public float Bottom => Y + Height;

        /// <summary>
        /// Constructor to create a Bounding rectangle given the top left coordinate and edge lengths
        /// </summary>
        /// <param name="x">X value of Origin</param>
        /// <param name="y">Y value of origin</param>
        /// <param name="width">the width of rectangle</param>
        /// <param name="height">the height of rectangle</param>
        public BoundingRectangle(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Constructor to create Bounding rectangle from Vector position
        /// </summary>
        /// <param name="position">Position of rectangle</param>
        /// <param name="width">the width</param>
        /// <param name="height">the height</param>
        public BoundingRectangle(Vector2 position, float width, float height)
        {
            X = position.X;
            Y = position.Y;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Tests for a collision between this and another BoundingRectangle
        /// </summary>
        /// <param name="other">The other bounding rectangle</param>
        /// <returns>True for collision, false otherwise</returns>
        public bool CollidesWith(BoundingRectangle other)
        {
            return CollisionHelper.Collides(this, other);
        }

        /// <summary>
        /// Tests for collision between this a bounding circle
        /// </summary>
        /// <param name="other">The bounding circle</param>
        /// <returns>True for collision, false otherwise</returns>
        public bool CollidesWith(BoundingCircle other)
        {
            return CollisionHelper.Collides(other, this);
        }
    }
}
