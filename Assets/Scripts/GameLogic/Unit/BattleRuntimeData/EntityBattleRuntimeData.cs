using System;
using GameLogic.Unit;
using GameLogic.Unit.ConfigData;
using UnityEngine;
using Utilities;

namespace GameLogic.Character.BattleRuntimeData
{
    [Serializable]
    public abstract class EntityBattleRuntimeData
    {
        public readonly string InstanceID;
        public readonly string EntityID;
        public Direction dir;
        public Vector2Int gridCoord;
        public Faction faction;

        protected EntityBattleRuntimeData(string instanceID, string entityID, Faction faction, Direction dir, Vector2Int gridCoord)
        {
            InstanceID = instanceID;
            EntityID = entityID;
            this.faction = faction;
            this.dir = dir;
            this.gridCoord = gridCoord;
        }
        
        public abstract EntityConfigData ConfigData { get; }
    }
}