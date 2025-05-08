using UnityEngine;
using Utilities;

namespace GameLogic.Unit
{
    public class BatchCharacterSpawnData
    {
        public readonly Faction Faction;
        public readonly CharacterSpawnData[] SpawnQueue;
        
        public BatchCharacterSpawnData(Faction faction, CharacterSpawnData[] data)
        {
            Faction = faction;
            SpawnQueue = data;
        }
    }

    public class CharacterSpawnData
    {
        public readonly CharacterData CharacterData;
        public Vector2Int GridCoord;
        public readonly Direction Direction;
        
        public CharacterSpawnData(CharacterData characterData, Vector2Int gridCoord, Direction direction)
        {
            CharacterData = characterData;
            GridCoord = gridCoord;
            Direction = direction;
        }
    }
}