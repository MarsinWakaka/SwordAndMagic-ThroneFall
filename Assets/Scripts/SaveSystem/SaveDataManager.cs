using System.IO;
using Player;
using SaveSystem;
using UnityEngine;

namespace Core.SaveSystem
{
    public class SaveDataManager : ISaveDataProvider
    {
        public const string PlayerDataPath = "playerData.json";
        
        public SaveData LoadPlayerData()
        {
            // Load player data from file or database
            // For example, using JSON serialization
            var json = File.ReadAllText("playerData.json");
            return JsonUtility.FromJson<SaveData>(json);
        }
        
        public SaveData SavePlayerData()
        {
            // Load player data from file or database
            // For example, using JSON serialization
            var json = File.ReadAllText("playerData.json");
            return JsonUtility.FromJson<SaveData>(json);
        }

        public SaveData CreateDefaultData()
        {
            throw new System.NotImplementedException();
        }
    }
}