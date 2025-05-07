using System;
using Core.Log;
using GameLogic.Unit.Controller;
using MyFramework.Utilities.Extensions;
using UnityEngine;

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
        
        public virtual void TakeEffect(CharacterUnitController owner)
        {
            Owner = owner;
            ownerInstanceID = owner.RuntimeData.InstanceID;
            BattleLogManager.Instance.Log($"{owner.CharacterRuntimeData.ConfigData.entityID}的[{BaseConfig.skillName}]生效");
        }
        
        public virtual string GetDescription()
        {
            return BaseConfig == null ? $"{skillID} data not found" : BaseConfig.GetSkillDescription(skillLevel);
        }
    }
}