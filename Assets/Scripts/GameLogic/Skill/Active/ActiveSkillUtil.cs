using GameLogic.Unit.BattleRuntimeData;
using UnityEngine;

namespace GameLogic.Skill.Active
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ActiveSkillUtil
    {
        /// 检查技能是否可以使用
        public static bool CanUseSkill(ActiveSkillInstance skillInstance, CharacterBattleRuntimeData runtimeData)
        {
            if (skillInstance == null || runtimeData == null)
            {
                Debug.LogError("Skill instance or runtimeData is null.");
                return false;
            }

            if (skillInstance.IsCooldown()) return false;
            if (!skillInstance.IsSkillPointEnough(runtimeData)) return false;

            // 冷却完成 技能点足够
            return true;
        }
        
        /// 判断一个角色能否使用该技能攻击到目标
        public static bool CanAttackTarget(ActiveSkillInstance skillInstance, Vector3Int casterCoord, Vector3Int targetCoord)
        {
            if (skillInstance == null)
            {
                Debug.LogError("Skill instance is null.");
                return false;
            }
            
            // 检查技能范围
            var attackRange = skillInstance.GetAttackRange();

            // 可以攻击
            return true;
        }
    }
}