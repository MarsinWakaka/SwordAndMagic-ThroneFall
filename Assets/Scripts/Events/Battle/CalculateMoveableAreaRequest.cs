using System;
using System.Collections.Generic;
using System.Linq;
using GameLogic.Character.BattleRuntimeData;
using GameLogic.Grid.Path;
using GameLogic.Unit.BattleRuntimeData;
using MyFramework.Utilities;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace Events.Battle
{
    public class CalculateMoveableAreaRequest : IEventArgs
    {
        public readonly CharacterBattleRuntimeData asker;
        
        public Action<MoveableAreaResult> onPathFindingComplete;

        public CalculateMoveableAreaRequest(CharacterBattleRuntimeData asker, Action<MoveableAreaResult> onPathFindingComplete)
        {
            this.asker = asker;
            this.onPathFindingComplete = onPathFindingComplete;
        }
    }

    public class MoveableAreaResult
    {
        public readonly List<PathTreeNode> pathTreeNodes;
        
        public MoveableAreaResult(List<PathTreeNode> pathTreeNodes)
        {
            this.pathTreeNodes = pathTreeNodes;
        }

        public bool TryGetPathWayTo(Vector2Int target, out PathTreeNode pathTreeNode)
        {
            pathTreeNode = pathTreeNodes.FirstOrDefault(node => node.Coord == target);
            return pathTreeNode != null;
        }
        
        public List<Vector2Int> ToList()
        {
            return pathTreeNodes.Select(node => node.Coord).ToList();
        }
    }
}