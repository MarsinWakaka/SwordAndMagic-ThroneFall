using System.Collections.Generic;
using Events.Battle;
using GameLogic.Grid;
using GameLogic.Unit.ConfigData;
using GameLogic.Unit.Controller;
using MyFramework.Utilities;
using UnityEngine;
using IServiceProvider = MyFramework.Utilities.IServiceProvider;

namespace GameLogic.Unit
{
    public interface IUnitManager : IServiceProvider
    {
        public List<EntityController> GetEntities(EntityType entityType);
        
        public List<T> GetEntities<T>(EntityType entityType) where T : EntityController;
        
        public int GetCount(EntityType entityType);

        public void MoveUnit(EntityController entity, Vector2Int oldCoord, Vector2Int newCoord);
    }
    
    public class UnitManager : MonoBehaviour, IUnitManager
    {
        private readonly Dictionary<EntityType, List<EntityController>> _unitDict = new();
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

        // private void OnUnitMove(UnitMoveEvent moveEvent)
        // {
        //     var oldCoord = moveEvent.OldPosition;
        //     var newCoord = moveEvent.TargetPosition;
        //     _gridManager.MoveUnit(moveEvent.entity, oldCoord, newCoord);
        // }
        
        public void MoveUnit(EntityController entity, Vector2Int oldCoord, Vector2Int newCoord)
        {
            // TODO 减少移动力
            var grid = ServiceLocator.Resolve<IGridManager>();
            grid?.MoveUnitFromAToB(entity, oldCoord, newCoord);
            // TODO 未来BUFF监听通知
            // EventBus.Channel(Channel.Battle).Publish(GameEvent.UnitMove, new UnitMoveEvent(this, oldGridCoord, newCoord));
        }
        
        private void OnCharacterSpawned(CharacterSpawnedEvent args)
        {
            // 处理单位生成事件
            var unit = args.Unit;
            var unitType = unit.RuntimeData.ConfigData.entityType;
            if (!_unitDict.ContainsKey(unitType))
            {
                _unitDict[unitType] = new List<EntityController>();
            }
            _unitDict[unitType].Add(unit);
            
            var gridCoord = GridManager.GetNearestEmptyGrid(unit.RuntimeData.gridCoord);
            if(!GridManager.TryInitializeUnitOnGrid(unit, gridCoord))
            {
                Debug.LogError($"Failed to initialize unit {unit} on grid {gridCoord}");
                OnCharacterDespawn(new CharacterDespawnEvent(unit));
            }
        }
        
        private void OnCharacterDespawn(CharacterDespawnEvent args)
        {
            // 处理单位销毁事件
            var unit = args.Unit;
            var unitType = unit.RuntimeData.ConfigData.entityType;
            if (_unitDict.ContainsKey(unitType))
            {
                _unitDict[unitType].Remove(unit);
                if (_unitDict[unitType].Count == 0)
                {
                    _unitDict.Remove(unitType);
                }
                // 如果是角色，则检查阵营是否为空
                // var unitFaction = unit.RuntimeData.faction;
                // if (unitFaction == Faction.Player)
                // {
                //     Debug.Log($"UnitManager: {unitType} unit wiped out.");
                //     EventBus.Channel(Channel.Gameplay).Publish(new FactionWipeEvent(unit.RuntimeData.faction));
                // }
                Debug.Log($"UnitManager: {unitType} unit remain count: {_unitDict[unitType].Count}");
            }
            
            var coord = unit.RuntimeData.gridCoord;
            ServiceLocator.Resolve<IGridManager>().GetGridController(coord).OnUnitLeave(unit);
        }

        public List<EntityController> GetEntities(EntityType entityType)
        {
            if (_unitDict.TryGetValue(entityType, out var unitList)) {
                return unitList;
            }
            _unitDict[entityType] = new List<EntityController>();
            return _unitDict[entityType];
        }

        public int GetCount(EntityType entityType)
        {
            if (_unitDict.TryGetValue(entityType, out var unitList))
            {
                return unitList.Count;
            }
            return 0;
        }

        public List<T> GetEntities<T>(EntityType entityType) where T : EntityController
        {
            if (_unitDict.TryGetValue(entityType, out var unitList))
            {
                var result = new List<T>();
                foreach (var unit in unitList)
                {
                    if (unit is T tUnit)
                    {
                        result.Add(tUnit);
                    }
                }
                return result;
            }
            return new List<T>();
        }
    }
}