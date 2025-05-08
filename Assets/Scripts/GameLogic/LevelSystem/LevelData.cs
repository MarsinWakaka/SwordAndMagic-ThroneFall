using System;
using System.Collections.Generic;
using System.Linq;
using GameLogic.Task;
using GameLogic.Unit;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

namespace GameLogic.LevelSystem
{
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

        [Header("网格数据")]
        public List<SpawnGridData> grids;
        
        [FormerlySerializedAs("deployPoints")]
        [Header("部署配置")]
        public int maxDeployCount;
        public List<Vector2Int> deployableGrids;
        
        // 决定是否允许玩家使用自己的角色
        public bool allowPlayerCharacters = true;
        [SerializeField] private List<CharacterPresentData> presetCharacters = new();
        public List<CharacterData> PresetCharacters => presetCharacters.Select(p => p.CharacterDataConfig).ToList();   // 返回一个新的列表，避免外部修改原始列表
        
        [FormerlySerializedAs("spawnRequests")] 
        [Header("角色生成配置")]
        public List<BatchCharacterSpawnPresentData> characterSpawnData;
        
        [Header("任务部分")]
        public List<BaseTask> tasks;

        public int reward;
        
        public int GetReward(short levelDataStarsEarned)
        {
            return reward * levelDataStarsEarned / 3;
        }
    }
}
