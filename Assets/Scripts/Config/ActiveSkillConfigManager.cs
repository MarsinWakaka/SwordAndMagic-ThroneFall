// using GameLogic.Skill.Active;
// using MyFramework.Utilities.Singleton;
// using UnityEngine;
//
// namespace Config
// {
//     public class ActiveSkillConfigManager : Singleton<ActiveSkillConfigManager>
//     {
//         private readonly SimpleConfigManager<ActiveSkillConfig> _configManager = new(LoadActiveConfig);
//
//         public ActiveSkillConfig GetConfig(string configId)
//         {
//             if (string.IsNullOrEmpty(configId))
//             {
//                 Debug.LogError("ActiveSkillConfigManager: configId is null or empty");
//                 return null;
//             }
//             
//             var config = _configManager.GetConfig(configId);
//             if (config == null)
//             {
//                 Debug.LogError($"ActiveSkillConfigManager: config not found for id {configId}");
//                 return null;
//             }
//
//             return config;
//         }
//         
//         private static ActiveSkillConfig LoadActiveConfig(string configId)
//         {
//             return Resources.Load<ActiveSkillConfig>($"Skills/Active/{configId}");
//         }
//     }
// }