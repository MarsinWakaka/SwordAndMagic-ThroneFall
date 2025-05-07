using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    [CreateAssetMenu(menuName = "UI/UIManagerConfig", order = 1)]
    public class UIManagerConfig : ScriptableObject
    {
        [Tooltip("UIManager配置文件")]
        public string configFilePath;
        
        [Tooltip("UIManager配置文件MD5值")]
        public string configFileMD5;
        
        [Tooltip("UIManager配置文件版本号")]
        public int configFileVersion;
        
        [Tooltip("UIManager配置文件更新地址")]
        public string configFileUpdateUrl;
        
        [Tooltip("UIManager配置文件更新版本号")]
        public int configFileUpdateVersion;
        
        public string panelPathRootPath;
    }
}