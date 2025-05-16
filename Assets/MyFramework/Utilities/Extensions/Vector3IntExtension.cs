using UnityEngine;

namespace MyFramework.Utilities.Extensions
{
    public static class Vector3IntExtension
    {
        public static Vector2Int ToVector2Int(this Vector3Int vector3Int)
        {
            return new Vector2Int(vector3Int.x, vector3Int.y);
        }
    }
}