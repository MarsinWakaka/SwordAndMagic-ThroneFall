using UnityEngine;

namespace GameLogic.Skill
{
    public abstract class BaseSkillConfig : ScriptableObject
    {
        public string SkillID => name;
        [Header("基本技能信息")] 
        public string skillName;
        public Sprite skillIcon;
        
        [Header("升级所需数据-第一级为解锁所需数据")]
        public int[] upgradeResourceCost;
        
        /// <summary>
        /// 获取技能升级所需资源
        /// </summary>
        /// <param name="level">当等级为0级，代表解锁所需要的资源</param>
        /// <returns></returns>
        public int GetUpgradeCost(int level)
        {
            if (level < 0 || level > upgradeResourceCost.Length)
            {
                Debug.LogError($"Invalid skill level: {level}, max level: {upgradeResourceCost.Length}");
                return -1;
            }
            return upgradeResourceCost[level];
        }
        
        // public GameObject[] skillVFXPrefab;
        // public AudioClip[] skillAudioClip;
        
        // TODO 解析系统，从而使描述可以从外部配置
        public abstract string GetSkillDescription(int level);
    }
}