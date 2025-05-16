using GameLogic.Grid;
using MyFramework.Utilities;
using UnityEngine;

namespace GameLogic.Unit
{
    public interface IEntity
    {
        public string FriendlyInstanceID();
        public string InstanceID();
        public Vector2Int Coordinate();
    }
}