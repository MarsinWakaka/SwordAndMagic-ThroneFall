using System;
using Events.Battle;
using GameLogic.LevelSystem;
using GameLogic.Map;
using GameLogic.Unit.BattleRuntimeData;
using GameLogic.Unit.Controller;
using MyFramework.Utilities;
using UnityEngine;

namespace GameLogic.Unit
{
    public class CharacterFactory : MonoBehaviour
    {
        [SerializeField] private Transform unitRoot;
        public string GetPrefabPath(string entityID) => $"Characters/{entityID}/Prefab";
        
        private void Awake()
        {
            unitRoot = transform;
        }
        
        private void OnEnable()
        {
            // 初始化单位管理器
            EventBus.Channel(Channel.Gameplay).Subscribe<SpawnCharacterEvent>(OnCharacterSpawn);
        }
        
        private void OnDisable()
        {
            EventBus.Channel(Channel.Gameplay).Unsubscribe<SpawnCharacterEvent>(OnCharacterSpawn);
        }

        private void OnCharacterSpawn(SpawnCharacterEvent events)
        {
            var faction = events.Faction;
            // 处理单位生成事件
            foreach (var context in events.SpawnQueue)
            {
                // 创建单位
                CreateUnit(faction, context);
            }
        }

        private void CreateUnit(Faction faction, CharacterSpawnData data)
        {
            var characterID = data.CharacterData.characterID;
            var fullPath = GetPrefabPath(characterID);
            var characterPrefab = Resources.Load<GameObject>(fullPath);
            if (characterPrefab == null)
            {
                Debug.LogError($"Character prefab {characterID} not found at path： {fullPath}");
                return;
            }

            var characterController = characterPrefab.GetComponent<CharacterUnitController>();
            if (characterController == null)
            {
                Debug.LogError($"Character prefab {characterID} does not have a CharacterController component.");
                return;
            }

            var unitInstance = Instantiate(characterController, unitRoot);
            var instanceID = Guid.NewGuid().ToString();
            var runtimeData = new CharacterBattleRuntimeData(instanceID, data.CharacterData, faction, data.GridCoord,
                data.Direction);
            unitInstance.Initialize(runtimeData);

            // 命名
            unitInstance.name = $"{faction}_{characterID}_{instanceID}";
        }
    }
}