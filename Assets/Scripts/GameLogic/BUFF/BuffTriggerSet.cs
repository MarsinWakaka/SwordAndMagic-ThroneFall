using System;
using GameLogic.Battle;
using GameLogic.GameAction.Attack;

namespace GameLogic.BUFF
{
    public class BuffTriggerSet
    {
        public event Action<AttackAction> OnPreDoDamage;
        public event Action<AttackAction> OnAfterDoDamage;
        
        public event Action<AttackAction> OnPreTakeDamage;
        public event Action<AttackAction> OnAfterTakeDamage;
        
        public void TriggerPreDoDamage(AttackAction attackAction)
        {
            OnPreDoDamage?.Invoke(attackAction);
        }
        
        public void TriggerAfterDoDamage(AttackAction attackAction)
        {
            OnAfterDoDamage?.Invoke(attackAction);
        }
        
        public void TriggerPreTakeDamage(AttackAction attackAction)
        {
            OnPreTakeDamage?.Invoke(attackAction);
        }
        
        public void TriggerAfterTakeDamage(AttackAction attackAction)
        {
            OnAfterTakeDamage?.Invoke(attackAction);
        }
        
        public void Clear()
        {
            OnPreDoDamage = null;
            OnAfterDoDamage = null;
            OnPreTakeDamage = null;
            OnAfterTakeDamage = null;
        }
    }
}