using System;
using GameLogic.BUFF;
using GameLogic.Skill.Passive;
using GameLogic.Unit.Controller;
using UnityEngine;

namespace GameLogic.Skill.Concrete
{
    [CreateAssetMenu(menuName = "Character/Elaina/Skill/ElainaPassiveConfig")]
    public class ElainaPassiveConfig : PassiveSkillConfig
    {
        [Serializable]
        public class ElainaPassive
        {
            // 一回合内最多触发次数
            public int maxTriggerCount;
        }
        
        public ElainaPassive[] data;

        public ElainaPassive GetData(int level)
        {
            if (level <= 0 || level > data.Length)
            {
                Debug.LogError($"ElainaPassiveConfig: Invalid level {level}");
                return null;
            }
            return data[level - 1];
        }

        public override string GetSkillDescription(int level)
        {
            return "初始技能点+1\n" +
                   $"回合开始时, 自身获得{GetData(level).maxTriggerCount}层的支援BUFF\n" +
                   "支援BUFF存在期间，若友方发起攻击后，对攻击目标追加支援攻击\n";
        }
        
        public override PassiveSkillInstance CreatePassiveSkillInstance(int level)
        {
            return new ElainaPassiveSkillInstance(SkillID, level, GetData(level).maxTriggerCount);
        }
    }
    
    [Serializable]
    public class ElainaPassiveSkillInstance : PassiveSkillInstance
    {
        public int maxTriggerCount;

        public ElainaPassiveSkillInstance(string passiveSkillId, int skillLevel, int maxTriggerCount)
            : base(passiveSkillId, skillLevel)
        {
            this.maxTriggerCount = maxTriggerCount;
        }

        // public override void Initialize(CharacterUnitController owner)
        // {
        //     base.Initialize(owner);
        //     // TODO 这里需要根据等级来设置
        //     OnTurnStart();
        //     Owner.CharacterRuntimeData.CurSkillPoint += 1;
        // }

        private int _takeEffectTime;
        
        public override void OnTurnStart()
        {
            _takeEffectTime++;
            if (_takeEffectTime == 1)
            {
                Owner.CharacterRuntimeData.CurSkillPoint += 1;
            }
            var runtimeData = Owner.CharacterRuntimeData;
            Debug.Log($"ElainaPassiveSkillInstance: OnTurnStart, maxTriggerCount: {maxTriggerCount}");
            runtimeData.BuffManager.AddBuff(BuffID.SupportAttack, 0 , maxTriggerCount);
        }
    }
}