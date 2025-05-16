using System;
using Config;
using Core.Log;
using GameLogic.BUFF.Concrete.Config;
using GameLogic.GameAction.Attack;
using GameLogic.Unit.Controller;
using UnityEngine;

namespace GameLogic.BUFF.Concrete.Instance
{
    [Serializable]
    public class DefenceUpBuffIns : BuffInstance
    {
        public DefenceUpBuffIns(string buffID, CharacterUnitController owner) 
            : base(buffID, owner)
        {
        }

        public DefenceUpBuff DefenceUpBuffConfig
        {
            get
            {
                if (_defenceUpBuff != null) return _defenceUpBuff;
                var buffConfig = BuffConfigManager.GetConfig(buffID);
                if (buffConfig is DefenceUpBuff coverBuff)
                {
                    _defenceUpBuff = coverBuff;
                    return _defenceUpBuff;
                }
                Debug.LogError($"DefenceUpBuffIns: BuffID {buffConfig} is not a DefenceUpBuff");
                return null;
            }
            set => _defenceUpBuff = value;
        }
        private DefenceUpBuff _defenceUpBuff;

        
        protected override void OnApplyBuffEffect()
        {
            base.OnApplyBuffEffect();
            var allyFactionBuffManager = BuffSystem.Instance.GetFactionBuffs(Owner.CharacterRuntimeData.faction);
            allyFactionBuffManager.FactionBuffTriggers.OnPreTakeDamage += OnBuffTriggered;
        }

        protected override void OnRemoveBuffEffect()
        {
            var allyFactionBuffManager = BuffSystem.Instance.GetFactionBuffs(Owner.CharacterRuntimeData.faction);
            allyFactionBuffManager.FactionBuffTriggers.OnPreTakeDamage -= OnBuffTriggered;
            base.OnRemoveBuffEffect();
        }

        private void OnBuffTriggered(AttackAction attackAction)
        {
            if (!IsDefenderSelf(attackAction)) return;  // 受击者需为自身
            
            BattleLogManager.Instance.Log($"[{Owner.FriendlyInstanceID()}] 触发了 [{BuffID.DefenseUp}]");
            foreach (var damageData in attackAction.DamageSegments)
            {
                damageData.Damage = Mathf.Max(damageData.Damage - DefenceUpBuffConfig.increaseAmount, 0);
            }
            
            // // 得到支援技能
            // var supportSkill = Owner.CharacterRuntimeData.ActiveSkills[0];
            // // TODO 判断攻击者是否在自身普通攻击射程范围内
            //
            // // BUFF 触发的支援行动为 Reactive 类型
            // var sKillSelectContext = new ActiveSkillSelectContext(Owner, attackAction.Attacker.GetGridController(), ActionType.Reactive);
            // var skillExecuteWrapper = new ActiveSkillExecuteContextWrapper(sKillSelectContext, supportSkill.Execute);
            // TimeLineManager.Instance.AddPerform(skillExecuteWrapper.Execute);
            
            ReduceStackCount(1);
        }
    }
}