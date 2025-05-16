using Config;
using MyFramework.Utilities.Singleton;
using UnityEngine;
using Utilities;

namespace GameLogic.Grid
{
    public class GridConfigManager : Singleton<GridConfigManager>
    {
        private readonly SimpleConfigManager<GridDataConfig> _configManager = new(LoadConfig);
        public GridDataConfig GetConfig(string configId) => _configManager.GetConfig(configId);
        
        private static GridDataConfig LoadConfig(string configId)
        {
            return Resources.Load<GridDataConfig>($"Grids/{configId}");
        }
    }
}