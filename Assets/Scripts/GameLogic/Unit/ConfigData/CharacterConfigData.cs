using System;
using System.Collections.Generic;
using GameLogic.Skill.Active;
using GameLogic.Skill.Passive;
using UnityEngine;

namespace GameLogic.Unit.ConfigData
{
    [Serializable]
    public class CharacterProperty
    {
        /// 角色基础生命值
        public int maxHp;

        /// 角色基础移动距离
        public int maxMoveRange;

        /// 角色初始技能点
        public int initSkillPoint;
    }
    
    [CreateAssetMenu(menuName = "Unit/CharacterData")]
    public class CharacterConfigData : EntityConfigData
    {

        [Header("人物升级数据")] 
        [SerializeField] private List<CharacterProperty> upgradeData;
        
        public int MaxLevel => upgradeData.Count;
        
        public CharacterProperty GetCharacterProperties(int level)
        {
            if (level < 1 || upgradeData.Count < level)
            {
                Debug.LogError($"Invalid character level: {level}");
                return upgradeData[Mathf.Clamp(level, 1, upgradeData.Count - 1)];
            }
            return upgradeData[level - 1];
        }
        
        // /// <summary>
        // /// 主动技能
        // /// </summary> 
        // public List<ActiveSkillConfig> activeSkills;
        //
        // /// <summary>
        // /// 被动技能
        // /// </summary>
        // public List<PassiveSkillConfig> passiveSkills;
    }
}