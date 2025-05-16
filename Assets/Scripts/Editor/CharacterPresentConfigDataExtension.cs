using System.Collections.Generic;
using Config;
using GameLogic.Unit;

namespace Editor
{
    [UnityEditor.CustomEditor(typeof(CharacterPresentData))]
    public class CharacterPresentConfigDataExtension : UnityEditor.Editor
    {
        // 增添一个通过扫描所有技能配置文件，自动填充技能ID列表的功能
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var characterPresentData = (CharacterPresentData)target;
            if (UnityEditor.EditorApplication.isPlaying)
            {
                return;
            }
            
            if (UnityEngine.GUILayout.Button("补全技能"))
            {
                var characterID = characterPresentData.name.Split('_')[0];
                var config = characterPresentData.CharacterDataConfig;
                
                var skillConfigs = SkillConfigManager.LoadAllConfigs();
                var existSKill = config.GetSkillLevelMap();
                
                var skillList = new List<SkillData>();
                foreach (var skillConfig in skillConfigs)
                {
                    if (skillConfig.SkillID.StartsWith(config.characterID))
                    {
                        if (!existSKill.ContainsKey(skillConfig.SkillID))
                        {
                            // 如果技能ID不在角色配置中，则添加 
                            skillList.Add(new SkillData(skillConfig.SkillID, 1));
                        }
                    }
                }
                // 添加技能
                config.skillsData.AddRange(skillList);
                
                UnityEditor.EditorUtility.SetDirty(characterPresentData);
            }
        }
    }
}