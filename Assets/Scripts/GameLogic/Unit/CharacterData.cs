using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace GameLogic.Unit
{
    /// <summary>
    /// [Purpose]
    /// 定义一个角色所需的所有信息
    /// </summary>
    [Serializable]
    public class SkillData
    {
        public string skillID;
        public int level;
        
        public SkillData(string skillID, int level)
        {
            this.skillID = skillID;
            this.level = level;
        }
    }
    
    /// <summary>
    /// [Purpose]
    /// 用于玩家角色数据持久化的类()
    /// </summary>
    [Serializable]
    public class CharacterData
    {
        // 角色名称
        public string characterID;
        
        // 角色等级
        public int level;
        
        // 技能等级
        public List<SkillData> skillsData = new();
        
        // 装备数据等等...
        
        public Dictionary<string, int> GetSkillLevelMap()
        {
            var skillLevel = new Dictionary<string, int>();
            foreach (var skill in skillsData)
            {
                skillLevel[skill.skillID] = skill.level;
            }
            return skillLevel;
        }
    }
}