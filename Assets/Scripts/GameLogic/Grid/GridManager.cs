using System;
using System.Collections.Generic;
using Events.Battle;
using GameLogic.Grid.Area;
using GameLogic.Grid.Path;
using GameLogic.Unit.Controller;
using MyFramework.Utilities;
using UnityEngine;
using Utilities;
using IServiceProvider = MyFramework.Utilities.IServiceProvider;

namespace GameLogic.Grid
{
    public interface IGridManager : IServiceProvider, IPathFinding
    {
        // public GridController GetGridController(int x, int y);
        /// <summary>
        /// 格子是否适合行走(不考虑上面是否有单位), 如果格子不存在或者不适合行走则返回false
        /// </summary>
        public bool IsWalkableTerrain(int x, int y);
        
        /// <summary>
        /// 格子是否可以移动(考虑上面是否有单位)
        /// </summary>
        public bool CanMoveTo(int x, int y);
        public int QueryHeight(int x, int y);
        // Vector2Int拓展
        public bool IsWalkableTerrain(Vector2Int gridCoord) => IsWalkableTerrain(gridCoord.x, gridCoord.y);
        public GridController GetGridController(Vector2Int gridCoord);
        public Vector3 GetWorldPosition(Vector2Int gridCoord);
        
        /// <summary>
        /// 将一个单位从一个格子移动到另一个格子，更新格子上的单位引用和单位的网格坐标和世界坐标位置
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="oldCoord"></param>
        /// <param name="newCoord"></param>
        public void MoveUnitFromAToB(EntityController entity, Vector2Int oldCoord, Vector2Int newCoord)
        {
            if (entity == null) return;
            if (oldCoord == newCoord) return;
            var oldGrid = GetGridController(oldCoord);
            var newGrid = GetGridController(newCoord);
            if (oldGrid == null || newGrid == null) return;
            if (newGrid.RuntimeData.EntitiesOnThis != null)
            {
                Debug.LogError($"Grid {newGrid.RuntimeData.GridCoord.Value} already has an entity " +
                               $"{newGrid.RuntimeData.EntitiesOnThis.FriendlyInstanceID()}");
                return;
            }

            try
            {
                oldGrid.OnUnitLeave(entity);
                newGrid.OnUnitEnter(entity);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to move unit from {oldCoord} to {newCoord}: {e.Message}");
                // 取消单位进入
                if (newGrid.RuntimeData.EntitiesOnThis == entity) newGrid.OnUnitLeave(entity);
                oldGrid.OnUnitEnter(entity);
            }
        }
        
        public bool TryInitializeUnitOnGrid(EntityController entity, Vector2Int gridCoord)
        {
            var grid = GetGridController(gridCoord);
            if (grid == null) return false;
            if (grid.RuntimeData.EntitiesOnThis != null)
            {
                Debug.LogWarning($"Grid {grid.RuntimeData.GridCoord.Value} already has an entity " +
                               $"{grid.RuntimeData.EntitiesOnThis.FriendlyInstanceID()}");
                return false;
            }
            grid.OnUnitEnter(entity);
            return true;
        }
        
        // 获取距离目标位置最近(包含自身)，没有实体占用的格子，但只考虑最近的四个方向，没有则返回原坐标
        public Vector2Int GetNearestEmptyGrid(Vector2Int targetCoord)
        {
            if (CanMoveTo(targetCoord.x, targetCoord.y)) return targetCoord;
            var x = targetCoord.x;
            var y = targetCoord.y;
            for (var i = 0; i < 4; i++)
            {
                var newCoord = Vector2Direction.FourDirections[i] + targetCoord;
                if (CanMoveTo(newCoord.x, newCoord.y))
                {
                    return newCoord;
                }
            }
            return targetCoord;
        }        
        
        /// <summary>
        /// 找到一个距离目标位置最近的坐标(不包含自身)，如果不存在则resultCoord为默认值
        /// </summary>
        /// <returns>返回智斗有空位置</returns>
        public bool TryGetNearestEmptyGridNoSelf(Vector2Int targetCoord, out Vector2Int resultCoord)
        {
            resultCoord = default;
            var x = targetCoord.x;
            var y = targetCoord.y;
            for (var i = 0; i < 4; i++)
            {
                var newCoord = Vector2Direction.FourDirections[i] + targetCoord;
                if (CanMoveTo(newCoord.x, newCoord.y))
                {
                    resultCoord = newCoord;
                    return true;
                }
            }
            return false;
        }
    }
    
    public class GridManager : MonoBehaviour, IGridManager
    {
        private readonly Dictionary<int, GridController> _grids = new();
        
        private GridFactory _gridFactory;
        private IPathFinding _pathFinding;
        private IAttackableAreaCalculator _attackAreaCalculator;

        private void Awake()
        {
            _gridFactory = GetComponent<GridFactory>();
            if (_gridFactory == null) Debug.LogError("GridFactory component not found on GridManager.");
            _pathFinding = new PathFindImpl(this);
            _attackAreaCalculator = new SimpleAttackableAreaCalculator(this);
        }

        private void OnEnable()
        {
            EventBus.Channel(Channel.Gameplay).Subscribe<LoadGridRequest>(HandleLoadGridRequest);
            EventBus.Channel(Channel.Gameplay).Subscribe<CalculateMoveableAreaRequest>(CalculateMoveableArea);
            EventBus.Channel(Channel.Gameplay).Subscribe<CalculateAttackableAreaRequest>(CalculateAttackArea);
        }

        private void OnDisable()
        {
            EventBus.Channel(Channel.Gameplay).Unsubscribe<LoadGridRequest>(HandleLoadGridRequest);
            EventBus.Channel(Channel.Gameplay).Unsubscribe<CalculateMoveableAreaRequest>(CalculateMoveableArea);
            EventBus.Channel(Channel.Gameplay).Unsubscribe<CalculateAttackableAreaRequest>(CalculateAttackArea);
        }

        private void HandleLoadGridRequest(LoadGridRequest loadRequest)
        {
            var newGrids = _gridFactory.Create(loadRequest.Records);
            foreach (var newGrid in newGrids)
            {
                var gridCoord = newGrid.RuntimeData.GridCoord.Value;
                var gridID = GetRestoreKey(gridCoord.x, gridCoord.y);
                if (!_grids.TryAdd(gridID, newGrid))
                {
                    Debug.LogError($"Grid {gridCoord} already exists in the dictionary.");
                    continue;
                }
            }
        }

        private void CalculateMoveableArea(CalculateMoveableAreaRequest request)
        {
            var startPos = request.asker.gridCoord;
            var moveRange = request.asker.CurMoveRange.Value;
            var allNodes = _pathFinding.CalculateAllMoveablePath(startPos, moveRange);
            request.onPathFindingComplete?.Invoke(new MoveableAreaResult(allNodes));
        }
        
        private void CalculateAttackArea(CalculateAttackableAreaRequest request)
        {
            var result = _attackAreaCalculator.Calculate(request.AttackParams);
            request.OnCalculateAttackableAreaCompleted?.Invoke(result);
        }

        #region 接口实现
        
        public GridController GetGridController(Vector2Int gridCoord)
        {
            var restoreKey = GetRestoreKey(gridCoord);
            if (!IsGridExist(restoreKey))
            {
                Debug.LogError($"Grid coordinate {gridCoord} is out of range.");
                return null;
            }
            return _grids[restoreKey];
        }

        public Vector3 GetWorldPosition(Vector2Int gridCoord)
        {
            var restoreKey = GetRestoreKey(gridCoord);
            if (!IsGridExist(restoreKey))
            {
                Debug.LogError($"Grid coordinate {gridCoord} is out of range.");
                return Vector3.zero;
            }
            return CoordinateConverter.CoordToWorldPos(_grids[restoreKey].RuntimeData.GridCoord.Value);
        }
        public bool IsGridExist(int storeKey) => _grids.ContainsKey(storeKey);

        public bool IsWalkableTerrain(int x, int y)
        {
            var key = GetRestoreKey(x, y);
            if (_grids.TryGetValue(key, out var grid))
            {
                return grid.IsWalkAble;
            }
            return false;
        }

        public bool CanMoveTo(int x, int y)
        {
            return IsWalkableTerrain(x, y) && _grids.TryGetValue(GetRestoreKey(x, y), out var grid) && grid.RuntimeData.EntitiesOnThis == null;
        }

        public int QueryHeight(int x, int y) => _grids.TryGetValue(GetRestoreKey(x, y), out var grid) ? grid.RuntimeData.GridCoord.Value.z : -1;

        #endregion

        // TODO 解决使用移位运算符的方式的可读性差的问题
        public static Vector2Int ParseRestoreKey(int gridID) => new Vector2Int(gridID / 1000, gridID % 1000);
        private static int GetRestoreKey(Vector2Int gridCoord) => gridCoord.x * 1000 + gridCoord.y;
        private static int GetRestoreKey(int x, int y) => x * 1000 + y;
        
        public List<Vector2Int> FindPath(Vector2Int a, Vector2Int b, bool includeStart)
        {
            return _pathFinding.FindPath(a, b, includeStart);
        }

        public List<Vector2Int> TryFindPathToCloseTarget(Vector2Int a, Vector2Int b, int distThreshold, bool includeStart = false)
        {
            return _pathFinding.TryFindPathToCloseTarget(a, b, distThreshold, includeStart);
        }

        public List<PathTreeNode> CalculateAllMoveablePath(Vector2Int startPos, int moveRange, bool includeStart = false)
        {
            return _pathFinding.CalculateAllMoveablePath(startPos, moveRange, includeStart);
        }
    }
}