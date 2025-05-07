using System;
using Config;
using Core.Log;
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
                var buffConfig = BuffConfigManager.GetConfig(BuffID);
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
                curStackCount = Mathf.Max(curStackCount + stackCount, DefenceUpBuffConfig.maxStackCount);
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
            
            BattleLogManager.Instance.Log($"[{Owner.FriendlyInstanceID()}] 触发了 [{BuffName.DefenseUp}]");
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