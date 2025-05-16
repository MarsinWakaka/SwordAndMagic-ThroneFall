// using GameLogic.Skill.Passive;
// using MyFramework.Utilities.Singleton;
// using UnityEngine;
// using Utilities;
//
// namespace Config
// {
//     public class PassiveSkillConfigManager : Singleton<PassiveSkillConfigManager>
//     {
//         private readonly SimpleConfigManager<PassiveSkillConfig> _configManager = new(LoadConfig);
//
//         public PassiveSkillConfig GetConfig(string configId) => _configManager.GetConfig(configId);
//
//         private static PassiveSkillConfig LoadConfig(string configId)
//         {
//             return Resources.Load<PassiveSkillConfig>($"Skills/Passive/{configId}");
//         }
//     }
// }