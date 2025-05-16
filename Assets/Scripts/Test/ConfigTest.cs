using Config;
using UnityEngine;

namespace Test
{
    public class ConfigTest : MonoBehaviour
    {
        [SerializeField] private string skillConfigID;

        [ContextMenu("TestLoadActiveSkillConfig")]
        public void TestLoadActiveSkillConfig()
        {
            var skillConfig = SkillConfigManager.Instance.GetConfig(skillConfigID);
            if (skillConfig != null)
            {
                Debug.Log($"技能配置 {skillConfigID} 加载成功");
            }
            else
            {
                Debug.LogError($"技能配置 {skillConfigID} 加载失败");
            }
        }
        
        [ContextMenu("TestLoadBaseSkillConfig")]
        public void TestLoadBaseSkillConfig()
        {
            var skillConfig = SkillConfigManager.Instance.GetConfig(skillConfigID);
            if (skillConfig != null)
            {
                Debug.Log($"技能配置 {skillConfigID} 加载成功");
            }
            else
            {
                Debug.LogError($"技能配置 {skillConfigID} 加载失败");
            }
        }
    }
}