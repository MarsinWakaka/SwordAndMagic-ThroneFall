using System.Collections.Generic;
using Config;
using GameLogic.Unit.Controller;

namespace GameLogic.BUFF
{
    public class BuffManager
    {
        private readonly CharacterUnitController _owner;
        private readonly Dictionary<string, BuffInstance> _buffs = new();

        public BuffManager(CharacterUnitController owner)
        {
            _owner = owner;
        }

        public void AddBuff(string buffID, int stackCount)
        {
            if (_buffs.TryGetValue(buffID, out var existingBuff))
            {
                existingBuff.StackBuff(stackCount);
            }
            else
            {
                var buffInstance = BuffConfigManager.GetConfig(buffID).CreateBuffInstance(_owner);
                buffInstance.StackBuff(stackCount);
                _buffs.Add(buffID, buffInstance);
            }
        }
        
        public void OnTurnStart()
        {
            foreach (var buff in _buffs.Values)
            {
                buff.OnTurnStart();
            }
        }
        
        public void OnTurnEnd()
        {
            foreach (var buff in _buffs.Values)
            {
                buff.OnTurnEnd();
            }
        }
    }
}