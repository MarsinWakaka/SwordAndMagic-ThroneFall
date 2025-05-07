using GameLogic.BUFF;
using UnityEngine;
using Utilities;

namespace Config
{
    public static class BuffConfigManager
    {
        private static readonly SimpleConfigManager<Buff> ConfigManager = new(LoadConfig);

        private static Buff LoadConfig(string configId)
        {
            var fullPath = $"Buffs/{configId}";
            var buff = Resources.Load<Buff>(fullPath);
            if (buff == null)
            {
                Debug.LogError($"未找到配置: {configId} at {fullPath}");
                return null;
            }
            return buff;
        }

        public static Buff GetConfig(string configId) => ConfigManager.GetConfig(configId);
        
        // private static Dictionary<string, Buff> _configCache;
        // public static T GetConfig<T>(string configID) where T : Buff
        // {
        //     T result;
        //     if (_configCache.TryGetValue(configID, out var config))
        //     {
        //         if (config is not T castBuff)
        //         {
        //             Debug.LogError($"配置类型不匹配: From {config.GetType()} To {typeof(T)}");
        //             return null;
        //         }
        //         result = castBuff;
        //     }
        //     else
        //     {
        //         result = Resources.Load<T>($"Buffs/{configID}");
        //         if (result == null)
        //         {
        //             Debug.LogError($"未找到配置: {configID}");
        //             return null;
        //         }
        //         _configCache.Add(configID, result);
        //     }
        //     return result;
        // }
    }
}