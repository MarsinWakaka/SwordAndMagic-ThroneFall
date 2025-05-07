using System.Collections.Generic;
using GameLogic.Skill;
using GameLogic.Skill.Active;
using GameLogic.Skill.Passive;
using MyFramework.Utilities.Singleton;
using UnityEngine;

namespace Config
{
    public class BaseSkillConfigManager : Singleton<BaseSkillConfigManager>
    {
        private const string ActiveSkillConfigPath = "Skills/Active/";
        private const string PassiveSkillConfigPath = "Skills/Passive/";
        
        // 加入缓存
        private readonly Dictionary<string, BaseSkillConfig> skillConfigCache = new();
        
        // 优先去寻找是否是主动技能、如果没有找到再去寻找被动技能 
        public BaseSkillConfig GetSkillConfig(string skillID)
        {
            if (string.IsNullOrEmpty(skillID))
            {
                Debug.LogError("配置ID不能为空");
                return null;
            }
            var skillConfig = LoadFromCache(skillID);
            if (skillConfig != null) return skillConfig;
            var fullPath = $"{ActiveSkillConfigPath}{skillID}";
            // 先去主动技能路径寻找
            skillConfig = Resources.Load<ActiveSkillConfig>(fullPath);
            if (skillConfig == null)
            {
                Debug.Log($"Active skill config not found in {fullPath}, trying passive skill config");
                // 如果主动技能没有找到，再去被动技能路径寻找
                fullPath = $"{PassiveSkillConfigPath}{skillID}";
                skillConfig = Resources.Load<PassiveSkillConfig>(fullPath);
                if (skillConfig == null)
                {
                    Debug.LogError($"Passive skill config not found in {fullPath}");
                    return null;
                }
            }
            
            // 加入缓存 
            skillConfigCache.TryAdd(skillID, skillConfig);
            return skillConfig;
        }

        private BaseSkillConfig LoadFromCache(string skillName)
        {
            return skillConfigCache.GetValueOrDefault(skillName);
        }
    }
}