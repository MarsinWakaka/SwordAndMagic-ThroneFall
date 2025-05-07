using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic.Unit
{
    /// <summary>
    /// [Purpose]
    /// 定义一个角色所需的所有信息
    /// </summary>
    [Serializable]
    public struct SkillData
    {
        public string skillID;
        public int level;
    }
    
    [Serializable]
    [CreateAssetMenu(fileName = "new PresentData", menuName = "Character/PresentData", order = 1)]
    public class CharacterData : ScriptableObject
    {
        // 角色名称
        public string characterID;
        
        // 角色等级
        public int level;
        
        // 主动技能等级
        public List<SkillData> activeSkillsData = new();
        // 被动技能等级
        public List<SkillData> passiveSkillsData = new();
        
        // 装备数据等等...
    }
}