using UnityEngine;

namespace Utilities
{
    public static class DirectionUtil
    {
        public static Direction GetDirection(Vector2Int from, Vector2Int to)
        {
            if (from.x < to.x) return Direction.Right;
            if (from.x > to.x) return Direction.Left;
            if (from.y < to.y) return Direction.Up;
            if (from.y > to.y) return Direction.Down;
            Debug.LogError($"Invalid direction from {from} to {to}, return Up by default");
            return Direction.Up;
        }
        
        public static Direction GetDirectionNew(Vector2Int from, Vector2Int to)
        {
            var delta = to - from;
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            {
                if (delta.x > 0) return Direction.Right;
                if (delta.x < 0) return Direction.Left;
            }
            else
            {
                if (delta.y > 0) return Direction.Up;
                if (delta.y < 0) return Direction.Down;
            }
            Debug.LogError($"Invalid direction from {from} to {to}, return Up by default");
            return Direction.Up;
        }
    }
}