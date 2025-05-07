using System.Collections.Generic;
using GameLogic.Map;
using Player;
using UnityEngine;

namespace GameLogic.LevelSystem
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] private Transform levelSlotsParent;
        private LevelSlot[] _levelSlots;
        
        private readonly LevelDataLoader _levelDataLoader = new();
        
        private void Start()
        {
            _levelSlots = levelSlotsParent.GetComponentsInChildren<LevelSlot>();
            var validator = new HashSet<string>();
            foreach (var levelSlot in _levelSlots)
            {
                var levelID = levelSlot.levelID;
                
                if (levelID.StartsWith("测试"))
                {
                    levelSlot.Initialize(levelID, 0, true);
                    continue;
                }
                
                if (string.IsNullOrEmpty(levelID))
                {
                    Debug.LogError($"关卡槽 {levelSlot.name} 的关卡ID为空");
                    continue;
                }
                
                if (!validator.Add(levelID))
                {
                    Debug.LogError($"关卡槽 {levelSlot.name} 的关卡ID {levelID} 重复");
                    continue;
                }

                // 获取关卡文件
                if (LevelDataLoader.TryLoadLevelData(levelID, out var levelData))
                {
                    if (PlayerDataManager.Instance.TryGetLevelProgressData(levelID, out var levelPassedData))
                    {
                        // 获取关卡通关数据
                        levelSlot.Initialize(levelData.mapName, levelPassedData.starsEarned, true);
                    }
                    else
                    {
                        levelSlot.Initialize(levelData.mapName, 0, false);
                    }
                }
                else
                {
                    Debug.LogError($"关卡 {levelSlot.levelID} 的数据不存在");
                }
            }
        }
    }
}