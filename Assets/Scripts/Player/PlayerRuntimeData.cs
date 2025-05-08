using System;
using System.Collections.Generic;
using GameLogic.Unit;
using SaveSystem;
using UnityEngine;

namespace Player
{
    public class PlayerRuntimeData
    {
        // 玩家所拥有的角色
        public Dictionary<string, CharacterData> OwnedCharacters = new();
        
        // 用于升级的资源
        // 关卡通关情况
        public Dictionary<string, LevelProgressData> LevelPassed = new();

        private int _resource;
        private event Action<int> ResourceChangedCallback;

        public int Resources
        {
            get => _resource;
            set
            {
                if (_resource == value) return;
                _resource = value;
                ResourceChangedCallback?.Invoke(_resource);
            }
        }

        public void AddListenerOnResourcesChanged(Action<int> action)
        {
            ResourceChangedCallback += action;
        }

        public void RemoveListenerOnResourcesChanged(Action<int> action)
        {
            ResourceChangedCallback -= action;
        }

        /// <summary>
        /// 从存档数据中初始化玩家数据
        /// </summary>
        public PlayerRuntimeData(SaveData saveData)
        {
            // 初始化玩家数据
            OwnedCharacters = new Dictionary<string, CharacterData>();
            foreach (var character in saveData.ownedCharacters)
            {
                OwnedCharacters[character.characterID] = character;
            }
            
            Resources = saveData.resources;
            foreach (var level in saveData.levelPassedData)
            {
                LevelPassed[level.levelID] = level;
            }
            
            Debug.Log($"PlayerRuntimeData initialized with {OwnedCharacters.Count} characters and {LevelPassed.Count} levels, Resources: {Resources}");
        }
        
        public void LoadFromSave(SaveData saveData)
        {
            Resources = saveData.resources;
            // 初始化玩家数据
            OwnedCharacters.Clear();
            foreach (var character in saveData.ownedCharacters)
            {
                OwnedCharacters[character.characterID] = character;
            }
            LevelPassed.Clear();
            foreach (var level in saveData.levelPassedData)
            {
                LevelPassed[level.levelID] = level;
            }
            Debug.Log($"PlayerRuntimeData initialized with {OwnedCharacters.Count} characters and {LevelPassed.Count} levels, Resources: {Resources}");
        }
        
        /// <summary>
        /// 导出为存档数据
        /// </summary>
        public SaveData ToSaveData()
        {
            // 将玩家数据转换为存档数据
            var saveData = new SaveData
            {
                resources = Resources,
                ownedCharacters = new List<CharacterData>(),
                levelPassedData = new List<LevelProgressData>()
            };
            foreach (var characters in OwnedCharacters.Values)
            {
                saveData.ownedCharacters.Add(characters);
            }
            foreach (var level in LevelPassed)
            {
                saveData.levelPassedData.Add(new LevelProgressData(level.Key, level.Value.starsEarned));
            }
            Debug.Log($"PlayerRuntimeData exported with {saveData.ownedCharacters.Count} characters and {saveData.levelPassedData.Count} levels, Resources: {saveData.resources}");
            return saveData;
        }
    }
}