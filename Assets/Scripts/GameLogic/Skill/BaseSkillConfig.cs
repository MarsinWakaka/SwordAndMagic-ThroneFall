using UnityEngine;

namespace GameLogic.Skill
{
    public abstract class BaseSkillConfig : ScriptableObject
    {
        [Header("基本技能信息")]
        public string skillID;
        public string skillName;
        public Sprite skillIcon;
        
        // TODO 解析系统，从而使描述可以从外部配置
        public abstract string GetSkillDescription(int level);
    }
}