using GameLogic.Map;
using GameLogic.Unit;
using MyFramework.Utilities;

namespace Events.Battle
{
    /// <summary>
    /// 角色生成事件参数(只需要ID 和 位置)
    /// </summary>
    public class SpawnCharacterEvent : IEventArgs
    {
        public readonly Faction Faction;
        public readonly BaseCharacterSpawnData[] SpawnQueue;
        
        public SpawnCharacterEvent(Faction faction, BaseCharacterSpawnData[] data)
        {
            Faction = faction;
            SpawnQueue = data;
        }
        
        public SpawnCharacterEvent(CharacterSpawnData characterSpawnData)
        {
            Faction = characterSpawnData.faction;
            SpawnQueue = characterSpawnData.spawnQueue;
        }
    }
}