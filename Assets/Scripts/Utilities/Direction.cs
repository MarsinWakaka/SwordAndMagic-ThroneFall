using UnityEngine;

namespace Utilities
{
    public enum Direction
    {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3
    }
    
    public class Vector2Direction
    {
        public static readonly Vector2Int[] FourDirections =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };
    }
}