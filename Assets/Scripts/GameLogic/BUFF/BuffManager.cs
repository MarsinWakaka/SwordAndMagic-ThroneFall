using System.Collections.Generic;
using Config;
using GameLogic.Unit.Controller;
using UnityEngine;

namespace GameLogic.BUFF
{
    /// <summary>
    /// TODO 移动到CharacterUnitController中，而不是RuntimeData中
    /// </summary>
    public class BuffManager
    {
        private readonly CharacterUnitController _owner;
        private readonly Dictionary<string, BuffInstance> _buffs = new();

        public BuffManager(CharacterUnitController owner)
        {
            _owner = owner;
        }
        
        public List<BuffInstance> GetAllBuffs()
        {
            return new List<BuffInstance>(_buffs.Values);
        }

        public void RemoveAll()
        {
            foreach (var buff in _buffs.Values)
            {
                buff.RemoveBuff();
            }
            _buffs.Clear();
        }
        
        public void RemoveBuff(string buffID)
        {
            _buffs.Remove(buffID, out var buffInstance);
            buffInstance.RemoveBuff();
        }

        public void AddBuff(string buffID, int duration, int stackCount)
        {
            // stack和duration都为0时，直接返回
            if (stackCount <= 0 && duration <= 0)
            {
                Debug.LogWarning($"BuffManager: Buff {buffID} stackCount and duration are {duration} and {stackCount}, no need to add.");
                return;
            }
            if (!_buffs.ContainsKey(buffID))
            {
                var buffInstance = BuffConfigManager.GetConfig(buffID).CreateBuffInstance(_owner);
                _buffs.Add(buffID, buffInstance);
            }
            var existingBuff = _buffs[buffID];
            
            if (duration > 0) existingBuff.AddDuration(duration);
            if (stackCount > 0) existingBuff.AddStackBuff(stackCount);
        }
        
        public void OnBigTurnStart()
        {
            foreach (var buff in _buffs.Values)
            {
                buff.OnBigTurnStart();
            }
        }
        
        public void OnBigTurnEnd()
        {
            foreach (var buff in _buffs.Values)
            {
                buff.OnBigTurnEnd();
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