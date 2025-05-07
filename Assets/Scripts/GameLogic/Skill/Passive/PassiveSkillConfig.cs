using System;
using Config;
using GameLogic.Unit.Controller;
using MyFramework.Utilities.Extensions;
using UnityEngine;

namespace GameLogic.Skill.Passive
{
    public abstract class PassiveSkillConfig : BaseSkillConfig
    {
        public abstract PassiveSkillInstance CreatePassiveSkillInstance(int level);
    }
    
    [Serializable]
    public abstract class PassiveSkillInstance : BaseSkillInstance
    {
        protected PassiveSkillInstance(string passiveSkillId, int skillLevel)
        : base(passiveSkillId, skillLevel)
        {
        }
        
        // public override void TakeEffect(CharacterUnitController owner)
        // {
        //     $"{Owner.CharacterRuntimeData.ConfigData.entityID}的[{PassiveConfig.skillName}]生效".LogWithColor(Color.yellow);
        // }
        public override BaseSkillConfig BaseConfig => PassiveConfig;
        
        public PassiveSkillConfig PassiveConfig => PassiveSkillConfigManager.Instance.GetConfig(skillID);
    }
}