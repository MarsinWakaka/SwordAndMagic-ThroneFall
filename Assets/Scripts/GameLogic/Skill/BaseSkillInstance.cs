using System;
using Core.Log;
using GameLogic.Unit.Controller;

namespace GameLogic.Skill
{
    [Serializable]
    public abstract class BaseSkillInstance
    {
        public string skillID;
        public int skillLevel;
        public string ownerInstanceID;
        public CharacterUnitController Owner { get;private set; }

        public abstract BaseSkillConfig BaseConfig { get; }
        
        protected BaseSkillInstance(string skillID, int skillLevel)
        {
            this.skillID = skillID;
            this.skillLevel = skillLevel;
        }
        
        public virtual void Initialize(CharacterUnitController owner)
        {
            Owner = owner;
            ownerInstanceID = owner.RuntimeData.InstanceID;
            BattleLogManager.Instance.Log($"{owner.CharacterRuntimeData.ConfigData.EntityID}的[{BaseConfig.skillName}]初始化");
        }
        
        /// <summary>
        /// 回合刷新时调用
        /// </summary>
        public abstract void OnTurnStart();
        
        public virtual string GetDescription()
        {
            return BaseConfig == null ? $"{skillID} data not found" : BaseConfig.GetSkillDescription(skillLevel);
        }
    }
}