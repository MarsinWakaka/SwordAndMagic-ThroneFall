using System.IO;
using MyFramework.Utilities;
using UnityEngine;

namespace SaveSystem
{
    public interface ISaveDataProvider : IServiceProvider
    {
        public SaveData CreateDefaultData();
        
        public SaveData LoadPlayerData()
        {
            var json = File.ReadAllText("playerData.json");
            return JsonUtility.FromJson<SaveData>(json);
        }
        
        public void SavePlayerData(SaveData saveData)
        {
            var json = JsonUtility.ToJson(saveData);
            File.WriteAllText("playerData.json", json);
        }
    }
}