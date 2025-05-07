using GameLogic.Unit.Controller;
using MyFramework.Utilities;
using UnityEngine;

namespace Events.Battle
{
    public class UnitMoveEvent : IEventArgs
    {
        public readonly EntityController entity;
        public readonly Vector2Int OldPosition;
        public readonly Vector2Int TargetPosition;
        
        public UnitMoveEvent(EntityController entity, Vector2Int oldPosition, Vector2Int targetPosition)
        {
            entity = entity;
            OldPosition = oldPosition;
            TargetPosition = targetPosition;
        }
    }
}