using System;
using System.Linq;
using UnityEngine;
using Utilities;

namespace GameLogic.Unit
{
    [Serializable]
    public class BatchCharacterSpawnPresentData
    {
        public Faction faction;
        public BaseCharacterSpawnPresentData[] spawnQueue;
        
        public BatchCharacterSpawnPresentData(Faction faction, BaseCharacterSpawnPresentData[] data)
        {
            this.faction = faction;
            spawnQueue = data;
        }
        
        public BatchCharacterSpawnData ConvertToCharacterSpawnData()
        {
            return new BatchCharacterSpawnData(faction, spawnQueue.Select(s => s.ConvertToBaseCharacterSpawnData()).ToArray());
        }
    }

    [Serializable]
    public class BaseCharacterSpawnPresentData
    {        
        public CharacterPresentData characterPresentData;
        public Vector2Int gridCoord;
        public Direction direction;
        
        public BaseCharacterSpawnPresentData(CharacterPresentData characterPresentData, Vector2Int gridCoord, Direction direction)
        {
            this.characterPresentData = characterPresentData;
            this.gridCoord = gridCoord;
            this.direction = direction;
        }
        
        public CharacterSpawnData ConvertToBaseCharacterSpawnData()
        {
            return new CharacterSpawnData(characterPresentData.CharacterDataConfig, gridCoord, direction);
        }
    }

}