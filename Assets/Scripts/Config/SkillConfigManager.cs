using System.Collections.Generic;
using GameLogic.Skill;
using GameLogic.Skill.Active;
using MyFramework.Utilities.Singleton;
using UnityEngine;

namespace Config
{
    public class SkillConfigManager : Singleton<SkillConfigManager>
    {
        private const string SkillConfigPath = "Skills/";
        // private const string PassiveSkillConfigPath = "Skills/Passive/";
        
        // 加入缓存
        private readonly Dictionary<string, BaseSkillConfig> _skillConfigCache = new();
        
        /// <summary>
        /// 获取配置表，优先从缓存中获取
        /// </summary>
        /// <param name="skillID"></param>
        /// <returns></returns>
        public BaseSkillConfig GetConfig(string skillID)
        {
            if (string.IsNullOrEmpty(skillID))
            {
                Debug.LogError("配置ID不能为空");
                return null;
            }
            var skillConfig = LoadFromCache(skillID);
            if (skillConfig != null) return skillConfig;
            // 先去主动技能路径寻找
            skillConfig = LoadConfig(skillID);
            
            // 加入缓存 
            _skillConfigCache.TryAdd(skillID, skillConfig);
            return skillConfig;
        }

        private BaseSkillConfig LoadFromCache(string skillName)
        {
            // Default -> null
            return _skillConfigCache.GetValueOrDefault(skillName);
        }
        
        /// <summary>
        /// 加载配置
        /// </summary>
        /// <param name="skillID"></param>
        /// <returns></returns>
        public static BaseSkillConfig LoadConfig(string skillID)
        {
            if (string.IsNullOrEmpty(skillID))
            {
                Debug.LogError("配置ID不能为空");
                return null;
            }
            var fullPath = $"{SkillConfigPath}{skillID}";
            var skillConfig = Resources.Load<BaseSkillConfig>(fullPath);
            if (skillConfig == null)
            {
                Debug.LogError($"Skill config not found in {fullPath}");
                return null;
            }
            return skillConfig;
        }

        public static BaseSkillConfig[] LoadAllConfigs()
        {
            var skillConfigs = Resources.LoadAll<BaseSkillConfig>(SkillConfigPath);
            if (skillConfigs == null || skillConfigs.Length == 0)
            {
                Debug.LogError($"No skill configs found in {SkillConfigPath}");
                return null;
            }

            return skillConfigs;
        }
        
        public ActiveSkillConfig GetActiveSkillConfig(string skillID)
        {
            var skillConfig = GetConfig(skillID);
            if (skillConfig == null)
            {
                Debug.LogError($"Skill config with ID {skillID} not found.");
                return null;
            }
            if (skillConfig is ActiveSkillConfig activeSkillConfig)
            {
                return activeSkillConfig;
            }
            Debug.LogError($"Skill config with ID {skillID} is not an active skill config.");
            return null;
        }
    }
}