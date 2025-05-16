using System;
using System.Collections;
using System.Collections.Generic;
using GameLogic.Unit;
using GameLogic.Unit.Controller;
using MyFramework.Utilities.Singleton;

namespace GameLogic.BUFF
{
    public class QueryBuffTriggerContext
    {
        // 询问者
        public CharacterUnitController Asker;
        public string TriggerTime;
    }

    public class FactionBuffs
    {
        public BuffTriggerSet FactionBuffTriggers { get; } = new();
        public Dictionary<string, BuffTriggerSet> LocalBuffs { get; } = new();
    }

    public class BuffSystem : SceneSingleton<BuffSystem>
    {
        private readonly Dictionary<Faction, FactionBuffs> _factionBuffs = new()
        {
            { Faction.Player, new FactionBuffs() },
            { Faction.Enemy, new FactionBuffs() }
        };
        
        public FactionBuffs GetFactionBuffs(Faction faction)
        {
            if (_factionBuffs.TryGetValue(faction, out var factionBuffs))
            {
                return factionBuffs;
            }
            else
            {
                throw new Exception($"Faction {faction} not found in BuffSystem.");
            }
        }

        public IEnumerator TriggerBuff(QueryBuffTriggerContext context)
        {
            yield return null;
        }
        
        public void ClearBuffs()
        {
            foreach (var factionBuffs in _factionBuffs.Values)
            {
                factionBuffs.FactionBuffTriggers.Clear();
                factionBuffs.LocalBuffs.Clear();
            }
        }
    }
}