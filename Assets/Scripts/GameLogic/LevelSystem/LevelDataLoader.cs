using GameLogic.LevelSystem;
using UnityEngine;

namespace GameLogic.Map
{
    public class LevelDataLoader
    {
        private static string MapPath => "Maps/"; 
        
        public static bool TryLoadLevelData(string levelID, out LevelData levelData)
        {
            // 加载地图
            levelData = Resources.Load<LevelData>($"{MapPath}{levelID}/LevelData");
            return levelData != null;
        }
    }
}