using UnityEngine;

namespace MyFramework.Utilities.Extensions
{
    public static class Vector2IntExtension
    {
        public static Vector2Int Rotate(float angle)
        {
            var radian = angle * Mathf.Deg2Rad;
            var x = Mathf.RoundToInt(Mathf.Cos(radian));
            var y = Mathf.RoundToInt(Mathf.Sin(radian));
            return new Vector2Int(x, y);
        }

        /// <summary>
        /// 默认顺时针旋转90度
        /// </summary>
        public static Vector2Int Rotate90Degree(this Vector2Int vector2Int, bool isClockwise)
        {
            if (isClockwise)
            {
                return new Vector2Int(vector2Int.y, -vector2Int.x);
            }
            else
            {
                return new Vector2Int(-vector2Int.y, vector2Int.x);
            }
        }
        
        public static Vector2Int Rotate180Degree(this Vector2Int vector2Int)
        {
            return new Vector2Int(-vector2Int.x, -vector2Int.y);
        }
        
        public static bool IsSameOnXYAxis(this Vector2Int vector2Int, Vector3Int other)
        {
            return vector2Int.x == other.x && vector2Int.y == other.y;
        }
        
        public static int ManhattanDistance(this Vector2Int vector2Int, Vector2Int other)
        {
            return Mathf.Abs(vector2Int.x - other.x) + Mathf.Abs(vector2Int.y - other.y);
        }
    }
}