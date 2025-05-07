using System.Collections.Generic;
using UnityEngine;

namespace GameLogic.Grid.Area
{
    public enum AreaType
    {
        /// <summary>
        /// 角色可移动范围
        /// </summary>
        Move,
        
        /// <summary>
        /// 角色可攻击范围
        /// </summary>
        Attackable,
        
        /// <summary>
        /// 技能范围
        /// </summary>
        Skill,
        
        /// <summary>
        /// 可部署范围
        /// </summary>
        Deployable,
    }
    
    public interface IAreaDisplay
    {
        public AreaType CanHandleAreaType { get; }
        public void Display(IEnumerable<Vector2Int> area);
        public void Hide();
    }
}