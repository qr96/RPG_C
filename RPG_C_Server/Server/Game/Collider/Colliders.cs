using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Server.Game.Collider
{
    public class RectCollider
    {
        public Vector2 Min { get; private set; }
        public Vector2 Max { get; private set; }

        Vector2 originMin;
        Vector2 originMax;

        public void SetCollider(Vector2 offset, float width, float height)
        {
            originMin = new Vector2(-width / 2f, -height / 2f) + offset;
            originMax = new Vector2(width / 2f, height / 2f) + offset;

            Min = originMin;
            Max = originMax;
        }

        public void SetPosition(Vector2 pos)
        {
            Min = originMin + pos;
            Max = originMax + pos;
        }

        public bool IsIntersect(RectCollider rectangle)
        {
            if (Min.X == Max.X || Min.Y == Max.Y || rectangle.Min.X == rectangle.Max.X || rectangle.Min.Y == rectangle.Max.Y)
                return false;

            if (Min.X > rectangle.Max.X || rectangle.Min.X > Max.X)
                return false;

            if (Min.Y > rectangle.Max.Y || rectangle.Min.Y > Max.Y)
                return false;

            return true;
        }
    }
}
