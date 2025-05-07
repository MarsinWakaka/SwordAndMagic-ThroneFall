using System.Collections.Generic;
using GameLogic.Skill.Active;
using MyFramework.Utilities.Extensions;
using UnityEngine;

namespace MyFramework.Algorithm
{
    public class AttackUtil
    {
        /// <summary>
        /// 根据敌人位置和攻击范围自动选择攻击位置，如果没有合适的位置则返回false
        /// </summary>
        /// <returns>如果可移动的距离中有可以攻击到目标位置的格子，则返回true，否则返回false</returns>
        public static bool TryCalculateAttackPosition(
            IEnumerable<Vector2Int> moveablePos,
            ActiveSkillConfig attackSkill,
            Vector2Int targetPos,
            out Vector2Int attackPosition)
        {
            attackPosition = default;
            foreach (var pos in moveablePos)
            {
                // 检查攻击范围
                var attackRange = attackSkill.attackRange;
                var dist = pos.ManhattanDistance(targetPos);
                if (dist < attackRange.x || dist > attackRange.y)
                    continue;

                {
                    // TODO 考虑高度差
                    // TODO 考虑技能范围

                    // 可以攻击到目标位置
                    attackPosition = pos;
                    return true;
                }
            }
            return false;
        }
    }
}