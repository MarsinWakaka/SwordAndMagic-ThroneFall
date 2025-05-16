using Config;
using Core.Log;
using GameLogic.BUFF.Concrete.Config;
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
                var config = BuffConfigManager.GetConfig(buffID);
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

        protected override void OnApplyBuffEffect()
        {
            base.OnApplyBuffEffect();
            // TODO 处理buff添加的情况
            var allyFactionBuffManager = BuffSystem.Instance.GetFactionBuffs(Owner.CharacterRuntimeData.faction);
            allyFactionBuffManager.LocalBuffs[Owner.CharacterRuntimeData.EntityID].OnPreDoDamage += OnBuffTriggered;
        }

        protected override void OnRemoveBuffEffect()
        {
            var allyFactionBuffManager = BuffSystem.Instance.GetFactionBuffs(Owner.CharacterRuntimeData.faction);
            allyFactionBuffManager.LocalBuffs[Owner.CharacterRuntimeData.EntityID].OnPreDoDamage -= OnBuffTriggered;
            base.OnRemoveBuffEffect();
        }
        
        private void OnBuffTriggered(AttackAction attackAction)
        {
            if (!IsDefenderSelf(attackAction)) return;
            
            BattleLogManager.Instance.Log($"[{Owner.FriendlyInstanceID()}] 触发了 [{BuffID.AttackUp}]");
            foreach (var damageData in attackAction.DamageSegments)
            {
                damageData.Damage += AttackUpBuffConfig.increaseAmount;
            }
            
            ReduceStackCount(1);
        }
    }
}