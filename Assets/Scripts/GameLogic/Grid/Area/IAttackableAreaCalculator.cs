using System.Collections.Generic;
using System.Linq;
using GameLogic.Skill.Active;
using GameLogic.Unit.BattleRuntimeData;
using UnityEngine;

namespace GameLogic.Grid.Area
{
    public class AttackableAreaResult
    {
        public readonly List<Vector2Int> Area;
        public bool IsValid;
        
        public AttackableAreaResult(List<Vector2Int> area, bool isValid)
        {
            Area = area;
            IsValid = isValid;
        }

        public bool IsInAttackableArea(Vector2Int gridCoord)
        {
            return Area.Any(coord => coord == gridCoord);
        }
        
        public bool IsInAttackableArea(Vector3Int gridCoord)
        {
            return Area.Any(coord => coord.x == gridCoord.x && coord.y == gridCoord.y);
        }
    }

    public struct AttackParam
    {
        // 攻击者位置
        public Vector2Int GridCoord;
        
        public Vector2Int AttackRange;

        public AttackParam(Vector2Int gridCoord, Vector2Int attackRange)
        {
            GridCoord = gridCoord;
            AttackRange = attackRange;
        }
    }

    public interface IAttackableAreaCalculator
    {
        AttackableAreaResult Calculate(AttackParam param);
    }
    
    public class SimpleAttackableAreaCalculator : IAttackableAreaCalculator
    {
        private readonly IGridManager _gridManager;

        public SimpleAttackableAreaCalculator(IGridManager gridManager)
        {
            _gridManager = gridManager;
        }
        
        public AttackableAreaResult Calculate(AttackParam param)
        {
            // 计算攻击范围
            var attackableArea = new List<Vector2Int>();
            // var skillData = skill.SKillData;
            var minAttackRange = param.AttackRange.x;
            var maxAttackRange = param.AttackRange.y;
            var gridCoord = param.GridCoord;
            
            // var gridSize = ServiceLocator.Resolve<IGridDataProvider>().GridsSize;
            var left = gridCoord.x - maxAttackRange;
            var right = gridCoord.x + maxAttackRange;
            var down = gridCoord.y - maxAttackRange;
            var up = gridCoord.y + maxAttackRange;
            
            for (var x = left; x <= right; x++)
            {
                for (var y = down; y <= up; y++)
                {
                    if (!_gridManager.IsGridExist(x, y)) continue;
                    var distance = Mathf.Abs(x - gridCoord.x) + Mathf.Abs(y - gridCoord.y);
                    if (distance >= minAttackRange && distance <= maxAttackRange)
                    {
                        attackableArea.Add(new Vector2Int(x, y));
                    }
                }
            }

            return new AttackableAreaResult(attackableArea, true);
        }
    }
}