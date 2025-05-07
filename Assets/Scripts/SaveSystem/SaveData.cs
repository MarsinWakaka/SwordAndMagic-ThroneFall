using System;
using System.Collections.Generic;
using GameLogic.Unit;

namespace SaveSystem
{
    [Serializable]
    public class LevelProgressData
    {
        // 关卡ID
        public string levelID;
        
        // 获得⭐️的数量
        public short starsEarned;

        public LevelProgressData(string levelID, short starsEarned)
        {
            this.levelID = levelID;
            this.starsEarned = starsEarned;
        }
    }
    
    /// <summary>
    /// 存档数据，用于序列化
    /// </summary>
    [Serializable]
    public class SaveData
    {
        // 用于升级的资源
        public int resources;
        
        // 玩家所拥有的角色
        public List<CharacterData> ownedCharacters = new();
        
        // 关卡通关情况
        public List<LevelProgressData> levelPassedData = new();
    }
}