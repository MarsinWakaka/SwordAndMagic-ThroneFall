using System;
using System.Collections.Generic;
using UnityEngine;

namespace Config
{
    public class SimpleConfigManager<T> where T : ScriptableObject
    {
        private readonly Dictionary<string, T> _configCache = new();

        private readonly Func<string ,T> _onLoadConfig;

        public SimpleConfigManager(Func<string, T> onLoadConfig)
        {
            _onLoadConfig = onLoadConfig;
        }

        public T GetConfig(string configId)
        {
            if (string.IsNullOrEmpty(configId))
            {
                Debug.LogError("配置ID不能为空");
                return null;
            }
            if (!_configCache.TryGetValue(configId, out var config))
            {
                config = _onLoadConfig(configId);
                if (config == null)
                {
                    Debug.LogError($"未找到配置: {configId}");
                    return null;
                }
                _configCache.Add(configId, config);
            }
            return config;
        }
    }
}