using UnityEngine;

namespace GameLogic.Unit.ConfigData
{
    public enum EntityType
    {
        None = 0,
        Character = 1,
        Object = 2,
    }
    
    public abstract class EntityConfigData : ScriptableObject
    {
        public string entityID;
        public string unitName;
        public Sprite icon;
        public EntityType entityType;
        public bool blockMovement;
        
        [Header("描述")]
        [TextArea]
        public string description;
    }
}