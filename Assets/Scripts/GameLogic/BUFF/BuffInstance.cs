using System;
using Config;
using GameLogic.GameAction.Attack;
using GameLogic.Unit.Controller;
using UnityEngine;

namespace GameLogic.BUFF
{
    [Serializable]
    public abstract class BuffInstance
    {
        #region 序列化
        
        // 轻量级配置
        public readonly string BuffID;

        public int curStackCount;
        
        #endregion
        
        // Lazy loading
        public Buff BuffConfig
        {
            get
            {
                if (_buff == null) _buff = BuffConfigManager.GetConfig(BuffID);
                return _buff;
            }
            set => _buff = value;
        }
        
        private Buff _buff;
        protected CharacterUnitController Owner;

        protected BuffInstance(string buffID, CharacterUnitController owner)
        {
            BuffID = buffID;
            Owner = owner;
        }
        
        public abstract void StackBuff(int stackCount);
        
        public virtual void OnTurnStart(){}
        public virtual void OnTurnEnd(){}
        public virtual void OnBigTurnStart(){}
        public virtual void OnBigTurnEnd(){}

        protected abstract void OnAddBuffEffect();
        protected abstract void OnRemoveBuffEffect();

        public override string ToString()
        {
            return $"BUFF ID : {BuffID} " +
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