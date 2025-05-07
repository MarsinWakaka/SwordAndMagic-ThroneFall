using Config;
using MyFramework.Utilities.Singleton;
using UnityEngine;
using Utilities;

namespace GameLogic.Grid
{
    public class GridConfigManager : Singleton<GridConfigManager>
    {
        private readonly SimpleConfigManager<StaticGridData> _configManager = new(LoadConfig);
        public StaticGridData GetConfig(string configId) => _configManager.GetConfig(configId);
        
        private static StaticGridData LoadConfig(string configId)
        {
            return Resources.Load<StaticGridData>($"Grids/{configId}");
        }
    }
}