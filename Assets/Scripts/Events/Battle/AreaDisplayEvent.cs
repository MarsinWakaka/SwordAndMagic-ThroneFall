using System.Collections.Generic;
using GameLogic.Grid.Area;
using MyFramework.Utilities;
using UnityEngine;

namespace Events.Battle
{
    public class AreaDisplayEvent : IEventArgs
    {
        /// <summary>
        /// 区域类型
        /// </summary>
        public readonly AreaType AreaType;
        
        /// <summary>
        /// 需要显示的坐标
        /// </summary>
        public readonly IEnumerable<Vector2Int> Coords;
        
        /// <summary>
        /// 是否先隐藏旧的区域
        /// </summary>
        public readonly bool HideOldFirst;
        
        public AreaDisplayEvent(AreaType areaType, IEnumerable<Vector2Int> coords, bool hideOldFirst = false)
        {
            AreaType = areaType;
            Coords = coords;
            HideOldFirst = hideOldFirst;
        }
    }
}