using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GameLogic.Task;
using GameLogic.Unit;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

namespace GameLogic.Map
{
    [Serializable]
    public class CharacterSpawnData
    {
        public Faction faction;
        public BaseCharacterSpawnData[] spawnQueue;
        
        public CharacterSpawnData(Faction faction, BaseCharacterSpawnData[] data)
        {
            this.faction = faction;
            spawnQueue = data;
        }
    }

    [Serializable]
    public class BaseCharacterSpawnData
    {
        public CharacterData characterData;
        
        // public int spawnCountDown;
        // public string entityID;
        public Vector2Int gridCoord;
        public Direction direction;
        
        public BaseCharacterSpawnData(CharacterData characterData, Vector2Int gridCoord, Direction direction)
        {
            this.characterData = characterData;
            this.gridCoord = gridCoord;
            this.direction = direction;
        }
    }
    
    [Serializable]
    public class SpawnGridData
    {
        public string cellID;            
        public Vector2Int gridCoord;
        public int height;
    }
    
    [CreateAssetMenu(fileName = "LevelData", menuName = "GamePlay/LevelData", order = 1)]
    public class LevelData  : ScriptableObject
    {
        [Header("关卡ID")]
        public string mapID;
        public string mapName;
        
        [Header("剧情")]
        public bool hasDialogue;
        public string dialogueId;

        [Header("地图")]
        public List<SpawnGridData> grids;
        
        [FormerlySerializedAs("deployPoints")]
        [Header("部署")]
        public List<Vector2Int> deployableGrids;
        public int maxDeployCount;
        
        // 决定是否允许玩家使用自己的角色
        public bool allowPlayerCharacters;
        [SerializeField] private List<CharacterData> presetCharacters = new();
        public List<CharacterData> PresetCharacters => presetCharacters.ToList();   // 返回一个新的列表，避免外部修改原始列表
        
        [FormerlySerializedAs("spawnRequests")] 
        [Header("实体生成")]
        public List<CharacterSpawnData> characterSpawnData;
        
        [Header("任务部分")]
        public List<BaseTask> tasks;

        public int reward;
        
        public int GetReward(short levelDataStarsEarned)
        {
            return reward * levelDataStarsEarned / 3;
        }
    }
}
