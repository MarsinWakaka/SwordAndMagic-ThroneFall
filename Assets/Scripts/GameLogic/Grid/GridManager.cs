using System.Collections.Generic;
using Events;
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
        public bool IsGridExist(int x, int y);
        public int QueryHeight(int x, int y);
        // Vector2Int拓展
        public bool IsGridExist(Vector2Int gridCoord) => IsGridExist(gridCoord.x, gridCoord.y);
        public GridController GetGridController(Vector2Int gridCoord);
        public Vector3 GetWorldPosition(Vector2Int gridCoord);
        
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
            oldGrid.OnUnitLeave(entity);
            newGrid.OnUnitEnter(entity);
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
            var grid = GetGridController(targetCoord);
            if (grid == null) return targetCoord;
            if (grid.RuntimeData.EntitiesOnThis == null) return targetCoord;
            var gridCoord = grid.RuntimeData.GridCoord.Value;
            var x = gridCoord.x;
            var y = gridCoord.y;
            for (var i = 0; i < 4; i++)
            {
                var newCoord = Vector2Direction.FourDirections[i] + targetCoord;
                if (IsGridExist(newCoord.x, newCoord.y))
                {
                    var newGrid = GetGridController(newCoord);
                    if (newGrid.RuntimeData.EntitiesOnThis == null)
                    {
                        return newCoord;
                    }
                }
            }
            return targetCoord;
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
            EventBus.Channel(Channel.Gameplay).Subscribe<LoadGridRequest>(OnLoadGrid);
            EventBus.Channel(Channel.Gameplay).Subscribe<CalculateMoveableAreaRequest>(CalculateMoveableArea);
            EventBus.Channel(Channel.Gameplay).Subscribe<CalculateAttackableAreaRequest>(CalculateAttackArea);
        }

        private void OnDisable()
        {
            EventBus.Channel(Channel.Gameplay).Unsubscribe<LoadGridRequest>(OnLoadGrid);
            EventBus.Channel(Channel.Gameplay).Unsubscribe<CalculateMoveableAreaRequest>(CalculateMoveableArea);
            EventBus.Channel(Channel.Gameplay).Unsubscribe<CalculateAttackableAreaRequest>(CalculateAttackArea);
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

        private void OnLoadGrid(LoadGridRequest loadRequest)
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
        public bool IsGridExist(int x, int y) => _grids.ContainsKey(GetRestoreKey(x, y));
        public int QueryHeight(int x, int y) => _grids.TryGetValue(GetRestoreKey(x, y), out var grid) ? grid.RuntimeData.GridCoord.Value.z : -1;

        public static Vector2Int ParseRestoreKey(int gridID) => new Vector2Int(gridID / 1000, gridID % 1000);
        private static int GetRestoreKey(Vector2Int gridCoord) => gridCoord.x * 1000 + gridCoord.y;
        private static int GetRestoreKey(int x, int y) => x * 1000 + y;
        
        
        public List<Vector2Int> FindPath(Vector2Int start, Vector2Int end, bool includeStart, bool includeEnd)
        {
            return _pathFinding.FindPath(start, end, includeStart, includeEnd);
        }

        public List<PathTreeNode> CalculateAllMoveablePath(Vector2Int startPos, int moveRange)
        {
            return _pathFinding.CalculateAllMoveablePath(startPos, moveRange);
        }
    }
}