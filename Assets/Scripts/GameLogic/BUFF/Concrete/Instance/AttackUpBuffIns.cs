using Config;
using Core.Log;
using GameLogic.GameAction.Attack;
using GameLogic.Unit.Controller;
using UnityEngine;

namespace GameLogic.BUFF.Concrete.Instance
{
    public class AttackUpBuffIns : BuffInstance
    {
        public AttackUpBuff AttackUpBuffConfig
        {
            get
            {
                if (_defenceUpBuff != null) return _defenceUpBuff;
                var config = BuffConfigManager.GetConfig(BuffID);
                if (config is AttackUpBuff coverBuff)
                {
                    _defenceUpBuff = coverBuff;
                    return _defenceUpBuff;
                }
                Debug.LogError($"Buff Ins: BuffID {config} is not a {_defenceUpBuff.GetType()}");
                return null;
            }
            set => _defenceUpBuff = value;
        }
        private AttackUpBuff _defenceUpBuff;
        
        public AttackUpBuffIns(string buffID, CharacterUnitController owner) : base(buffID, owner)
        {
        }

        public override void StackBuff(int stackCount)
        {            
            if (curStackCount == 0)
            {
                // TODO 处理buff第一次添加的情况
                curStackCount = stackCount;
                // 监听事件，例如友军被攻击、回合开始、或者敌人攻击前
                OnAddBuffEffect();
            }
            else
            {
                // TODO 处理buff叠加的情况
                curStackCount = Mathf.Max(curStackCount + stackCount, AttackUpBuffConfig.maxStackCount);
            }
        }
        
        public void ReduceStackCount(int stackCount)
        {
            curStackCount -= stackCount;
            if (curStackCount <= 0)
            {
                // TODO 处理buff消失的情况
                OnRemoveBuffEffect();
            }
        }

        protected override void OnAddBuffEffect()
        {            
            // TODO 处理buff添加的情况
            var allyFactionBuffManager = BuffSystem.Instance.GetFactionBuffs(Owner.CharacterRuntimeData.faction);
            allyFactionBuffManager.FactionBuffTriggers.OnPreTakeDamage += OnBuffTriggered;
        }

        protected override void OnRemoveBuffEffect()
        {
            var allyFactionBuffManager = BuffSystem.Instance.GetFactionBuffs(Owner.CharacterRuntimeData.faction);
            allyFactionBuffManager.FactionBuffTriggers.OnPreTakeDamage -= OnBuffTriggered;
        }
        
        private void OnBuffTriggered(AttackAction attackAction)
        {
            if (!IsDefenderSelf(attackAction)) return;  // 受击者需为自身
            
            BattleLogManager.Instance.Log($"[{Owner.FriendlyInstanceID()}] 触发了 [{BuffName.AttackUp}]");
            foreach (var damageData in attackAction.DamageSegments)
            {
                damageData.Damage = damageData.Damage + AttackUpBuffConfig.increaseAmount;
            }
            
            ReduceStackCount(1);
        }
    }
}