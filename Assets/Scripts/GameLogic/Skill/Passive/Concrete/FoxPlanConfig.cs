using System;
using GameLogic.BUFF;
using GameLogic.Unit.BattleRuntimeData;
using GameLogic.Unit.Controller;
using UnityEngine;

namespace GameLogic.Skill.Passive.Concrete
{
    [CreateAssetMenu(menuName = "SkillConfig/Passive/FoxPlan")]
    public class FoxPlanConfig : PassiveSkillConfig
    {
        [Serializable]
        public class FoxPlanData
        {
            // 一回合内最多触发次数
            public int maxTriggerCount;
        }
        
        public FoxPlanData[] data;

        public FoxPlanData GetData(int level)
        {
            if (level < 0 || level > data.Length)
            {
                Debug.LogError($"FoxPlanPassive: Invalid level {level}");
                return null;
            }
            return data[level - 1];
        }

        public override string GetSkillDescription(int level)
        {
            return "初始技能点+3\n" +
                   $"回合开始时，为持有者添加一层防御提升BUFF\n";
        }
        
        public override PassiveSkillInstance CreatePassiveSkillInstance(int level)
        {
            return new FoxPlanPassiveSkillInstance(skillID, level, GetData(level).maxTriggerCount);
        }
    }
    
    [Serializable]
    public class FoxPlanPassiveSkillInstance : PassiveSkillInstance
    {
        public int maxTriggerCount;
        public int hasTriggerCount;

        public FoxPlanPassiveSkillInstance(string passiveSkillId, int skillLevel, int maxTriggerCount)
            : base(passiveSkillId, skillLevel)
        {
            this.maxTriggerCount = maxTriggerCount;
            hasTriggerCount = 0;
        }

        public override void TakeEffect(CharacterUnitController owner)
        {
            base.TakeEffect(owner);
            // TODO 这里需要根据等级来设置
            var runtimeData = Owner.CharacterRuntimeData;
            runtimeData.BuffManager.AddBuff(BuffName.DefenseUp, 1);
            // 初始技能点+3
            runtimeData.CurSkillPoint.Value = Math.Min(runtimeData.CurSkillPoint.Value + 3, CharacterBattleRuntimeData.MaxSkillPoint);
        }
    }
}