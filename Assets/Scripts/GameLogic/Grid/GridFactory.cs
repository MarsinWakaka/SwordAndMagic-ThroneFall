using System.Collections.Generic;
using GameLogic.LevelSystem;
using GameLogic.Map;
using UnityEngine;

namespace GameLogic.Grid
{
    public class GridFactory : MonoBehaviour
    {
        private Transform _gridsRoot;
        [SerializeField] private GridController gridPrefab;

        private void Awake()
        {
            _gridsRoot = transform;
        }
        
        public List<GridController> Create(List<SpawnGridData> grids)
        {
            var result = new List<GridController>();
            _gridsRoot = transform;
            Vector2 gridsSize = Vector2.zero;
            foreach (var grid in grids)
            {
                gridsSize.x = Mathf.Max(gridsSize.x, grid.gridCoord.x + 1);
                gridsSize.y = Mathf.Max(gridsSize.y, grid.gridCoord.y + 1);
            }
            var maxCol = (int) gridsSize.x;
            var maxRow = (int) gridsSize.y;
            
            foreach (var grid in grids)
            {
                var x = grid.gridCoord.x;
                var y = grid.gridCoord.y;
                var height = grid.height;
                
                var cell = Instantiate(gridPrefab, _gridsRoot);
                // cell.Initialize(cellData[x,y], new Vector3Int(x, y, height));
                var runtimeData = new RuntimeGridData(grid.cellID, new Vector3Int(x, y, height));
                cell.Initialize(runtimeData);
                result.Add(cell);
            }
            return result;
        }
        
        public void Create(List<SpawnGridData> grids, out int[,] cellHeight)
        {
            // Initialize the grid factory
            _gridsRoot = transform;
            Vector2 gridsSize = Vector2.zero;
            foreach (var grid in grids)
            {
                gridsSize.x = Mathf.Max(gridsSize.x, grid.gridCoord.x + 1);
                gridsSize.y = Mathf.Max(gridsSize.y, grid.gridCoord.y + 1);
            }
            var maxCol = (int) gridsSize.x;
            var maxRow = (int) gridsSize.y;
            
            // _gridsID = new int[maxCol, maxRow];
            cellHeight = new int[maxCol, maxRow];
            
            foreach (var grid in grids)
            {
                var x = grid.gridCoord.x;
                var y = grid.gridCoord.y;
                var height = grid.height;
                // _gridsID[grid.gridCoord.x, grid.gridCoord.y] = grid.cellID;
                cellHeight[x, y] = height;
                
                var cell = Instantiate(gridPrefab, _gridsRoot);
                // cell.Initialize(cellData[x,y], new Vector3Int(x, y, height));
                var runtimeData = new RuntimeGridData(grid.cellID, new Vector3Int(x, y, height));
                cell.Initialize(runtimeData);
            }
        }
    }
}