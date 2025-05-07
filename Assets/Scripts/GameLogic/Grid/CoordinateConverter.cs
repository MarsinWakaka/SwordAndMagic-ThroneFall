using UnityEngine;

namespace GameLogic.Grid
{
    public static class CoordinateConverter 
    {
        
        // TODO 缓存格子坐标和世界坐标的转换结果(同时合适时机清除缓存)
        
        // public static Vector3 ToGridWorldPos(Vector2Int gridCoord, int height)
        // {
        //     return new Vector3(
        //         gridCoord.x * 0.5f + gridCoord.y * 0.5f, 
        //         gridCoord.y * 0.25f - gridCoord.x * 0.25f + height * 0.25f,
        //         height);
        // }
        //
        // public static Vector3 ToGridWorldPos(Vector3Int gridCoord)
        // {
        //     return new Vector3(
        //         gridCoord.x * 0.5f + gridCoord.y * 0.5f, 
        //         gridCoord.y * 0.25f - gridCoord.x * 0.25f + gridCoord.z * 0.25f,
        //         gridCoord.z);
        // }
        
        // 格子坐标转世界坐标（以格子中心为基准）
        public static Vector3 CoordToWorldPos(Vector3Int gridCoord)
        {
            var pos = new Vector3(
                gridCoord.x * 0.5f + gridCoord.y * 0.5f, 
                gridCoord.y * 0.25f - gridCoord.x * 0.25f + gridCoord.z * 0.25f,
                -(gridCoord.z));
            return pos;
        }
    }
}