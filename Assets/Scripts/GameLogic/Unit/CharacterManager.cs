using System.Collections.Generic;
using Events.Battle;
using GameLogic.Grid;
using GameLogic.Unit.Controller;
using MyFramework.Utilities;
using MyFramework.Utilities.Extensions;
using UnityEngine;
using IServiceProvider = MyFramework.Utilities.IServiceProvider;

namespace GameLogic.Unit
{
    public interface ICharacterManager : IServiceProvider
    {
        public List<CharacterUnitController> GetEntities(Faction faction);

        public int GetCount(Faction entityType);

        /// <summary>
        /// 获取距离单位最近的同阵营单位
        /// </summary>
        /// <returns>如果场上没有其它同阵营单位，则返回null，否则则返回该单位</returns>
        public CharacterUnitController GetNearestAlly(CharacterUnitController character, bool includeSelf = false)
        {
            var unitList = GetEntities(character.CharacterRuntimeData.faction);
            if (unitList.Count == 0) return null;

            CharacterUnitController nearestUnit = null;
            var minDistance = float.MaxValue;
            foreach (var unit in unitList)
            {
                if (!includeSelf && unit == character) continue;
                var distance = character.Coordinate().ManhattanDistance(unit.Coordinate());
                if (distance < minDistance)
                {
                    nearestUnit = unit;
                    minDistance = distance;
                }
            }
            return nearestUnit;
        }

        public CharacterUnitController GetNearestHostile(CharacterUnitController character)
        {
            var unitList = GetEntities(character.CharacterRuntimeData.faction.Opposite());
            if (unitList.Count == 0) return null;

            CharacterUnitController nearestUnit = null;
            var minDistance = float.MaxValue;
            foreach (var unit in unitList)
            {
                var distance = character.Coordinate().ManhattanDistance(unit.Coordinate());
                if (distance < minDistance)
                {
                    nearestUnit = unit;
                    minDistance = distance;
                }
            }
            return nearestUnit;
        }
    }

    public class CharacterManager : MonoBehaviour, ICharacterManager
    {
        private readonly Dictionary<Faction, List<CharacterUnitController>> _unitDict = new();
        private List<CharacterUnitController> GetFactionOrCreateDefault(Faction faction)
        {
            if (_unitDict.TryGetValue(faction, out var unitList))
            {
                return unitList;
            }
            _unitDict[faction] = new List<CharacterUnitController>();
            return _unitDict[faction];
        }
        
        private IGridManager _gridManager;

        private IGridManager GridManager
        {
            get
            {
                _gridManager = ServiceLocator.Resolve<IGridManager>();
                if (_gridManager == null)
                {
                    Debug.LogError("GridDataProvider not found.");
                }

                return _gridManager;
            }
        }

        private void OnEnable()
        {
            // 初始化单位管理器
            EventBus.Channel(Channel.Gameplay).Subscribe<CharacterSpawnedEvent>(OnCharacterSpawned);
            EventBus.Channel(Channel.Gameplay).Subscribe<CharacterDespawnEvent>(OnCharacterDespawn);
            // EventBus.Channel(Channel.Battle).Subscribe<UnitMoveEvent>(GameEvent.UnitMove, OnUnitMove);
        }

        private void OnDisable()
        {
            EventBus.Channel(Channel.Gameplay).Unsubscribe<CharacterSpawnedEvent>(OnCharacterSpawned);
            EventBus.Channel(Channel.Gameplay).Unsubscribe<CharacterDespawnEvent>(OnCharacterDespawn);
            // EventBus.Channel(Channel.Battle).Unsubscribe<UnitMoveEvent>(GameEvent.UnitMove, OnUnitMove);
        }

        private void OnCharacterSpawned(CharacterSpawnedEvent args)
        {
            // 处理单位生成事件
            var unit = args.Unit;
            var faction = unit.CharacterRuntimeData.faction;
            GetFactionOrCreateDefault(faction).Add(unit);

            var gridCoord = GridManager.GetNearestEmptyGrid(unit.RuntimeData.gridCoord);
            if (!GridManager.TryInitializeUnitOnGrid(unit, gridCoord))
            {
                Debug.LogError($"Failed to initialize unit {unit} on grid {gridCoord}");
                OnCharacterDespawn(new CharacterDespawnEvent(unit));
            }
        }

        private void OnCharacterDespawn(CharacterDespawnEvent args)
        {
            // 处理单位销毁事件
            var unit = args.Unit;
            var unitType = unit.CharacterRuntimeData.faction;
            if (_unitDict.ContainsKey(unitType))
            {
                if (!_unitDict[unitType].Remove(unit))
                {
                    Debug.LogError($"Failed to remove unit {unit} from faction {unitType}");
                }
                if (_unitDict[unitType].Count == 0)
                {
                    _unitDict.Remove(unitType);
                    // EventBus.Channel(Channel.Gameplay).Publish(new FactionWipeEvent(unit.RuntimeData.faction));
                }
                else Debug.Log($"UnitManager: {unitType} unit remain count: {_unitDict[unitType].Count}");
            }

            var coord = unit.RuntimeData.gridCoord;
            ServiceLocator.Resolve<IGridManager>().GetGridController(coord).OnUnitLeave(unit);
        }

        public List<CharacterUnitController> GetEntities(Faction faction)
        {
            if (_unitDict.TryGetValue(faction, out var existList))
            {
                var unitList = new List<CharacterUnitController>(existList);
                return unitList;
            }
            return new List<CharacterUnitController>(0);
        }

        public int GetCount(Faction entityType)
        {
            return _unitDict.TryGetValue(entityType, out var unitList) ? unitList.Count : 0;
        }
    }
}