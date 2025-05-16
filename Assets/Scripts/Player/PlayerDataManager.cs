using System.Collections.Generic;
using System.Linq;
using Config;
using Core.SaveSystem;
using Events.Battle;
using Events.Global;
using GameLogic.Map;
using GameLogic.Unit;
using MyFramework.Utilities;
using MyFramework.Utilities.Singleton;
using SaveSystem;
using UnityEngine;

namespace Player
{
    public interface IPlayerDataProvider
    {
        public bool TryGetCharacterData(string characterID, out CharacterData characterData);
        
        public int Resources { get;}
        
        public int GetCharacterSkillLevel(string characterID, string skillID);
        
        // 查询玩家拥有的角色 [数据的拷贝]
        public List<CharacterData> GetAllOwnedCharacters();

        
        // 查询关卡数据
        public bool TryGetLevelProgressData(string levelID, out LevelProgressData levelData);
        public List<LevelProgressData> GetLevelPassedData();
    }
    
    public class PlayerDataManager : ThreadSafeMonoSingleton<PlayerDataManager>, IPlayerDataProvider
    {
        #region IPlayerDataProvider 实现
        
        // 该类实现了 IPlayerDataProvider 接口
        private PlayerRuntimeData _playerRuntimeData;

        private PlayerRuntimeData PlayerRuntimeData
        {
            get
            {
                if (_playerRuntimeData != null) return _playerRuntimeData;
                var saveDataProvider = ServiceLocator.Resolve<ISaveDataProvider>();
                _playerRuntimeData = new PlayerRuntimeData(saveDataProvider.CreateDefaultData());
                if (_playerRuntimeData == null)
                {
                    Debug.LogError("PlayerRuntimeData is null. Please check the initialization.");
                }
                return _playerRuntimeData;
            }
        }

        public bool TryGetCharacterData(string characterID, out CharacterData characterData)
        {
            if (PlayerRuntimeData.OwnedCharacters.TryGetValue(characterID, out characterData)) return true;
            Debug.LogError($"Character with ID {characterID} not found.");
            return false;
        }

        public int Resources => PlayerRuntimeData.Resources;
        public void AddListerOnResourcesChanged(System.Action<int> action)
        {
            PlayerRuntimeData.AddListenerOnResourcesChanged(action);
        }
        
        public void RemoveListerOnResourcesChanged(System.Action<int> action)
        {
            PlayerRuntimeData.RemoveListenerOnResourcesChanged(action);
        }

        public int GetCharacterSkillLevel(string characterID, string skillID)
        {
            if (PlayerRuntimeData.OwnedCharacters.TryGetValue(characterID, out var characterData))
            {
                var skill = characterData.skillsData.FirstOrDefault(s => s.skillID == skillID);
                if (skill != null)
                {
                    return skill.level;
                }
            }
            Debug.LogError($"Skill with ID {skillID} not found for character {characterID}.");
            return -1;
        }

        public List<CharacterData> GetAllOwnedCharacters()
        {
            return PlayerRuntimeData.OwnedCharacters.Values.ToList();
        }

        /// <summary>
        /// 验证角色是否拥有
        /// </summary>
        /// <param name="characterID"></param>
        /// <param name="skillData"></param>
        public bool TryUnlockSkill(string characterID, ref SkillData skillData)
        {
            if (PlayerRuntimeData.OwnedCharacters.TryGetValue(characterID, out var characterData))
            {
                var skillID = skillData.skillID;
                var hasUnlocked = characterData.skillsData.FindIndex(s => s.skillID == skillID) != -1;
                if (hasUnlocked)
                {
                    Debug.LogError($"Skill {skillID} already unlocked for character {characterID}.");
                    return false;
                }
                // 查看资源是否足够
                Debug.Log($"Unlocking skill {skillID} for character {characterID}.");
                var skillConfig = SkillConfigManager.Instance.GetConfig(skillID);
                var unlockCost = skillConfig.GetUpgradeCost(0);
                if (PlayerRuntimeData.Resources < unlockCost)
                {
                    Debug.Log($"Not enough resources to unlock skill {skillID}. Required: {unlockCost}, Available: {PlayerRuntimeData.Resources}");
                    return false;
                }
                // 扣除资源
                PlayerRuntimeData.Resources -= unlockCost;
                characterData.skillsData.Add(new SkillData(skillID, 1));
                skillData.level = 1;
                Debug.Log($"Skill {skillID} unlocked for character {characterID}. Remaining resources: {PlayerRuntimeData.Resources}");
                
                return true;
            }
            return false;
        }

        public int GetPlayerResources()
        {
            return PlayerRuntimeData.Resources;
        }

        public bool TryGetLevelProgressData(string levelID, out LevelProgressData levelData)
        {
            return PlayerRuntimeData.LevelPassed.TryGetValue(levelID, out levelData);
        }

        public List<LevelProgressData> GetLevelPassedData()
        {
            return PlayerRuntimeData.LevelPassed.Values.ToList();
        }

        #endregion

        
        protected override void WhenEnable()
        {
            EventBus.Channel(Channel.Global).Subscribe<CharacterUpgradeRequestEvent>(HandleCharacterUpgradeRequest);
            EventBus.Channel(Channel.Gameplay).Subscribe<LevelPassedEvent>(HandleLevelPassedEvent);
        }

        protected override void WhenDisable()
        {
            EventBus.Channel(Channel.Global).Unsubscribe<CharacterUpgradeRequestEvent>(HandleCharacterUpgradeRequest);
            EventBus.Channel(Channel.Gameplay).Unsubscribe<LevelPassedEvent>(HandleLevelPassedEvent);
        }

        #region 业务处理
        
        /// 通关事件处理
        private void HandleLevelPassedEvent(LevelPassedEvent evt)
        {
            // 处理关卡通关事件
            // TODO 【暂时去掉】，日后需要加回去
            // if (PlayerRuntimeData.LevelPassed.ContainsKey(evt.LevelID))
            // {
            //     Debug.Log($"Level {evt.LevelID} already passed.");
            //     return;
            // }
            
            // 更新关卡数据
            var levelData = new LevelProgressData(evt.LevelID, evt.Stars);
            PlayerRuntimeData.LevelPassed[evt.LevelID] = levelData;
            // 获取资源
            // 加入资源
            if (!LevelDataLoader.TryLoadLevelData(levelData.levelID, out var levelDataConfig))
            {
                Debug.LogError($"Level data for {levelData.levelID} not found.");
                return;
            }
            var reward = levelDataConfig.GetReward(levelData.starsEarned);
            PlayerRuntimeData.Resources += reward;
            
            // 发布更新数据
            Debug.Log($"Level {evt.LevelID} passed with {evt.Stars} stars.");
        }
        
        /// 角色升级请求处理
        private void HandleCharacterUpgradeRequest(CharacterUpgradeRequestEvent requestEvent)
        {
            // 验证角色ID
            if (!TryGetCharacterData(requestEvent.CharacterID, out var characterData)) return;
            // 验证目标大于当前等级
            if (requestEvent.TargetLevel <= characterData.level)
            {
                Debug.LogError($"Target level {requestEvent.TargetLevel} is less than or equal to current level {characterData.level}.");
                return;
            }
            
            // 验证目标等级小于最大等级
            if (requestEvent.TargetLevel > CharacterConfigManager.Instance.GetConfig(requestEvent.CharacterID).MaxLevel)
            {
                Debug.LogError($"Target level {requestEvent.TargetLevel} exceeds max level.");
                return;
            }
            
            // 验证资源
            var availableRes = PlayerRuntimeData.Resources;
            var needRes = CharacterUpgradeCost.GetAccumulatedCost(characterData.level, requestEvent.TargetLevel);
            if (availableRes < needRes)
            {
                Debug.LogError($"Not enough resources. Required: {needRes}, Available: {availableRes}");
                return;
            }
            
            // 更新数据
            characterData.level = requestEvent.TargetLevel;
            PlayerRuntimeData.Resources -= needRes;
            
            // 发布更新数据
            Debug.Log($"Character {requestEvent.CharacterID} upgraded to level {requestEvent.TargetLevel}" +
                      $". Remaining resources: {PlayerRuntimeData.Resources}");
            
            EventBus.Channel(Channel.Global).Publish(new CharacterUpgradeSuccessEvent(characterData));
        }
        
        #endregion

        public void SavePlayerData()
        {
            // TODO 保存玩家数据
            var saveDataProvider = ServiceLocator.Resolve<ISaveDataProvider>();
            saveDataProvider.SavePlayerData(PlayerRuntimeData.ToSaveData());
        }
        
        public void LoadPlayerData()
        {
            // TODO加载玩家数据
            var saveDataProvider = ServiceLocator.Resolve<ISaveDataProvider>();
            var playerData = saveDataProvider.LoadPlayerData();
            if (playerData != null)
            {
                PlayerRuntimeData.LoadFromSave(playerData);
            }
            else
            {
                Debug.LogError("Failed to load player data.");
            }
        }
    }
}