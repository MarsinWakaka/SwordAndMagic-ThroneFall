using GameLogic;
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
    }
}