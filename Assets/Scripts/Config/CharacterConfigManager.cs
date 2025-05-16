using GameLogic.Unit.ConfigData;
using MyFramework.Utilities.Singleton;
using UnityEngine;

namespace Config
{
    public class CharacterConfigManager : Singleton<CharacterConfigManager>
    {
        private SimpleConfigManager<CharacterConfigData> _configManager = new(LoadConfig);
        
        public CharacterConfigData GetConfig(string configId)
        {
            return _configManager.GetConfig(configId);
        }

        private static CharacterConfigData LoadConfig(string configId)
        {
            return Resources.Load<CharacterConfigData>($"Characters/ConfigData/{configId}");
        }
    }
}