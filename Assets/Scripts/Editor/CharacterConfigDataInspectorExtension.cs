using Config;
using GameLogic.Unit.ConfigData;

namespace Editor
{
    [UnityEditor.CustomEditor(typeof(CharacterConfigData))]
    public class CharacterConfigDataInspectorExtension : UnityEditor.Editor
    {
        // 增添一个通过扫描所有技能配置文件，自动填充技能ID列表的功能
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var characterConfigData = (CharacterConfigData)target;
            if (UnityEditor.EditorApplication.isPlaying)
            {
                return;
            }
            
            if (UnityEngine.GUILayout.Button("Scan Skills"))
            {
                characterConfigData.skillIDs.Clear();
                var skillConfigs = SkillConfigManager.LoadAllConfigs();
                foreach (var skillConfig in skillConfigs)
                {
                    if (skillConfig.SkillID.StartsWith(characterConfigData.EntityID))
                    {
                        characterConfigData.skillIDs.Add(skillConfig.SkillID);
                    } 
                }
                
                UnityEditor.EditorUtility.SetDirty(characterConfigData);
            }
        }
    }
}