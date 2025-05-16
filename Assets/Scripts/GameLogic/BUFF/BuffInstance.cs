using System;
using Config;
using GameLogic.GameAction.Attack;
using GameLogic.Unit.Controller;
using UnityEngine;
using UnityEngine.Serialization;

namespace GameLogic.BUFF
{
    [Serializable]
    public abstract class BuffInstance
    {
        #region 序列化
        
        // 轻量级配置
        public string buffID;

        public int curDuration;
        public int curStackCount;
        
        public int MaxStackCount => BuffConfig.maxStackCount;
        
        #endregion
        
        // Lazy loading
        public BuffConfig BuffConfig
        {
            get
            {
                if (_buff == null) _buff = BuffConfigManager.GetConfig(buffID);
                return _buff;
            }
            set => _buff = value;
        }
        
        private BuffConfig _buff;
        protected CharacterUnitController Owner;

        protected BuffInstance(string buffID, CharacterUnitController owner)
        {
            this.buffID = buffID;
            Owner = owner;
        }
        
        public void AddDuration(int durationValue)
        {
            // 表示该BUFF未启用持续时间
            if (BuffConfig.maxDuration == 0)
            {
                Debug.LogWarning($"Buff {buffID} does not have duration, cannot add duration.");
                return;
            }
            var notExist = curDuration == 0;
            curDuration = Mathf.Min(curDuration + durationValue, BuffConfig.maxDuration);
            if (!notExist || _isBuffApplied) return;
            OnApplyBuffEffect();
        }
        
        public void AddStackBuff(int stackCount)
        {
            // 表示该BUFF未启用叠加层数
            if (BuffConfig.maxStackCount == 0)
            {
                Debug.LogWarning($"Buff {buffID} does not have stack count, cannot add stack count.");
                return;
            }
            var notExist = curStackCount == 0;
            // TODO 处理buff叠加的情况
            curStackCount = Mathf.Min(curStackCount + stackCount, MaxStackCount);
            if (!notExist || _isBuffApplied) return;
            OnApplyBuffEffect();
        }
        
        public void ReduceStackCount(int stackCount)
        {
            if (BuffConfig.maxStackCount == 0)
            {
                Debug.LogWarning($"Buff {buffID} does not have stack count, cannot reduce stack count.");
                return;
            }
            curStackCount -= stackCount;
            if (curStackCount <= 0 && _isBuffApplied)
            {
                Owner.CharacterRuntimeData.BuffManager.RemoveBuff(buffID);
            }
        }

        public void ReduceDuration(int durationValue)
        {
            if (BuffConfig.maxDuration == 0)
            {
                Debug.LogWarning($"Buff {buffID} does not have duration, cannot reduce duration.");
                return;
            }
            curDuration -= durationValue;
            if (curDuration <= 0 && _isBuffApplied)
            {
                Owner.CharacterRuntimeData.BuffManager.RemoveBuff(buffID);
            }
        }

        private bool _isBuffApplied;
        /// <summary>
        /// BUFF Manager调用
        /// </summary>
        public void RemoveBuff()
        {
            if (!_isBuffApplied) return;
            // TODO 处理buff消失的情况
            OnRemoveBuffEffect();
        }
        
        public virtual void OnBigTurnStart(){}
        public virtual void OnBigTurnEnd(){}
        public virtual void OnTurnStart(){}
        public virtual void OnTurnEnd(){}


        protected virtual void OnApplyBuffEffect()
        {
            _isBuffApplied = true;
            // TODO 处理buff添加的情况
        }

        protected virtual void OnRemoveBuffEffect()
        {
            _isBuffApplied = false;
        }

        
        public string GetDurationText()
        {
            return curDuration == 0 ? "--" : curDuration.ToString();
        }
        
        // 堆叠层数
        public string GetStackCountText()
        {
            return curStackCount == 0 ? "--" : curStackCount.ToString();
        }

        public string GetDescription()
        {
            if (BuffConfig == null)
            {
                Debug.LogError($"BuffConfig is null for BuffID: {buffID}");
                return string.Empty;
            }
            return BuffConfig.description;
        }

        
        public override string ToString()
        {
            return $"BUFF ID : {buffID} " +
                   $"StackCount : {curStackCount}";
        }

        #region 实用函数

        protected bool IsActionActive(AttackAction action) => action.ActionType == ActionType.Active;
        
        protected bool IsActionReactive(AttackAction action) => action.ActionType == ActionType.Reactive;
        
        protected bool IsAttackerFromSameFaction(AttackAction action)
        {
            return Owner.CharacterRuntimeData.faction == action.Attacker.CharacterRuntimeData.faction;
        }
        
        protected bool IsDefenderFromSameFaction(AttackAction action)
        {
            return Owner.CharacterRuntimeData.faction == action.Attacker.CharacterRuntimeData.faction;
        }
        
        protected bool IsDefenderSelf(AttackAction action)
        {
            return action.Defender.GetInstanceID() == Owner.GetInstanceID();
        }
        
        protected bool IsAttackerSelf(AttackAction action)
        {
            return action.Attacker.GetInstanceID() == Owner.GetInstanceID();
        }

        #endregion
    }
}