using System;
using GameLogic.BUFF;
using GameLogic.Skill.Passive;
using UnityEngine;

namespace GameLogic.Skill.Concrete
{
    [CreateAssetMenu(menuName = "Character/Ais/Skill/AisPassiveConfig")]
    public class AisPassiveConfig : PassiveSkillConfig
    {
        [Serializable]
        public class AisPassiveData
        {
            // 一回合内最多触发次数
            public int maxTriggerCount;
        }
        
        public AisPassiveData[] data;

        public AisPassiveData GetData(int level)
        {
            if (level < 0 || level > data.Length)
            {
                Debug.LogError($"AisPassiveConfig: Invalid level {level}");
                return null;
            }
            return data[level - 1];
        }

        public override string GetSkillDescription(int level)
        {
            return $"回合开始时，为持有者添加{GetData(level).maxTriggerCount}层攻击提升BUFF";
        }
        
        public override PassiveSkillInstance CreatePassiveSkillInstance(int level)
        {
            return new AisPassiveSkillInstance(SkillID, level, GetData(level).maxTriggerCount);
        }
    }
    
    [Serializable]
    public class AisPassiveSkillInstance : PassiveSkillInstance
    {
        public int maxTriggerCount;

        public AisPassiveSkillInstance(string passiveSkillId, int skillLevel, int maxTriggerCount)
            : base(passiveSkillId, skillLevel)
        {
            this.maxTriggerCount = maxTriggerCount;
        }

        // public override void Initialize(CharacterUnitController owner)
        // {
        //     base.Initialize(owner);
        //     // TODO 这里需要根据等级来设置
        //     // OnTurnStart();
        //     // Owner.CharacterRuntimeData.CurSkillPoint += 3;
        // }
        
        // private int _takeEffectTime = 0;

        public override void OnTurnStart()
        {
            // _takeEffectTime++;
            // if (_takeEffectTime == 1)
            // {
            //     Owner.CharacterRuntimeData.CurSkillPoint += 1;
            // }
            // TODO 这里需要根据等级来设置
            var runtimeData = Owner.CharacterRuntimeData;
            runtimeData.BuffManager.AddBuff(BuffID.AttackUp,0, maxTriggerCount);
        }
    }
}