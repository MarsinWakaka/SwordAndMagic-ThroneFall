using System;
using Config;

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

        public override void OnTurnStart(){ }

        public override BaseSkillConfig BaseConfig => SkillConfigManager.Instance.GetConfig(skillID);

        // public PassiveSkillConfig PassiveConfig => PassiveSkillConfigManager.Instance.GetConfig(skillID);
    }
}