// using System.Collections.Generic;
// using GameLogic.Skill.Active;
// using GameLogic.Skill.Passive;
// using MyFramework.Utilities.Singleton;
// using UnityEngine;
//
// namespace GameLogic.Skill
// {
//     public class SkillConfigManager : MonoSingleton<SkillConfigManager>
//     {
//         private readonly Dictionary<string, ActiveSkillConfig> _activeSkillConfigCache = new();
//         
//         private const string ActiveSKillConfigPath = "Skills/ActiveSkillConfig";
//         
//         public ActiveSkillConfig GetActiveSkillConfig(string activeSkillId)
//         {
//             if (!_activeSkillConfigCache.TryGetValue(activeSkillId, out var activeSkillConfig))
//             {
//                 activeSkillConfig = Resources.Load<ActiveSkillConfig>($"{ActiveSKillConfigPath}/{activeSkillId}");
//                 if (activeSkillConfig == null)
//                 {
//                     Debug.LogError($"未找到技能配置: {activeSkillId}");
//                     return null;
//                 }
//                 _activeSkillConfigCache.Add(activeSkillId, activeSkillConfig);
//             }
//             return activeSkillConfig;
//         }
//         
//         private readonly Dictionary<string, PassiveSkillConfig> _passiveSkillConfigCache = new();
//         
//         private const string PassiveSkillConfigPath = "Skills/PassiveSkillConfig";
//         
//         public PassiveSkillConfig GetPassiveSkillConfig(string passiveSkillId)
//         {
//             if (!_passiveSkillConfigCache.TryGetValue(passiveSkillId, out var passiveSkillConfig))
//             {
//                 passiveSkillConfig = Resources.Load<PassiveSkillConfig>($"{PassiveSkillConfigPath}/{passiveSkillId}");
//                 if (passiveSkillConfig == null)
//                 {
//                     Debug.LogError($"未找到被动技能配置: {passiveSkillId}");
//                     return null;
//                 }
//                 _passiveSkillConfigCache.Add(passiveSkillId, passiveSkillConfig);
//             }
//             return passiveSkillConfig;
//         }
//     }
// }