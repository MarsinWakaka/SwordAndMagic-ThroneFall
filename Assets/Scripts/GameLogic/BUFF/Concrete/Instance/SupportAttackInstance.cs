using Config;
using Core.Log;
using GameLogic.BUFF.Concrete.Config;
using GameLogic.GameAction.Attack;
using GameLogic.Skill.Active;
using GameLogic.TimeLine;
using GameLogic.Unit.Controller;
using UnityEngine;

namespace GameLogic.BUFF.Concrete.Instance
{
    public class SupportAttackInstance : BuffInstance
    {
        public SupportAttackInstance(string buffID, CharacterUnitController owner) 
            : base(buffID, owner)
        {
        }

        public SupportAttackBuff SupportAttackBuffConfig
        {
            get
            {
                if (_supportAttackBuff != null) return _supportAttackBuff;
                var buffConfig = BuffConfigManager.GetConfig(buffID);
                if (buffConfig is SupportAttackBuff coverBuff)
                {
                    _supportAttackBuff = coverBuff;
                    return _supportAttackBuff;
                }
                Debug.LogError($"DefenceUpBuffIns: BuffID {buffConfig} is not a DefenceUpBuff");
                return null;
            }
            set => _supportAttackBuff = value;
        }
        private SupportAttackBuff _supportAttackBuff;

        
        protected override void OnApplyBuffEffect()
        {
            base.OnApplyBuffEffect();
            var allyFactionBuffManager = BuffSystem.Instance.GetFactionBuffs(Owner.CharacterRuntimeData.faction);
            allyFactionBuffManager.FactionBuffTriggers.OnAfterDoDamage += OnBuffTriggered;
        }

        protected override void OnRemoveBuffEffect()
        {
            var allyFactionBuffManager = BuffSystem.Instance.GetFactionBuffs(Owner.CharacterRuntimeData.faction);
            allyFactionBuffManager.FactionBuffTriggers.OnAfterDoDamage -= OnBuffTriggered;
            base.OnRemoveBuffEffect();
        }

        private void OnBuffTriggered(AttackAction attackAction)
        {
            // 条件判断
            // 只有主动攻击才会触发
            if (IsActionReactive(attackAction)) return;
            // 攻击者需为非自身的友方单位
            if (!IsAttackerFromSameFaction(attackAction) || IsAttackerSelf(attackAction)) return;
            
            // // 得到支援技能
            var supportSkill = Owner.CharacterRuntimeData.ActiveSkills[0]; 
            // TODO 判断攻击者是否在自身普通攻击射程范围内
            
            // BUFF 触发的支援行动为 Reactive 类型
            var sKillSelectContext = new ActiveSkillSelectContext(Owner, attackAction.Defender.GetGridController(), ActionType.Reactive);
            var skillExecuteWrapper = new ActiveSkillExecuteContextWrapper(sKillSelectContext, supportSkill.Execute);
            TimeLineManager.Instance.AddPerform(skillExecuteWrapper.Execute);
            BattleLogManager.Instance.Log($"[{Owner.FriendlyInstanceID()}] 触发了 [{BuffID.SupportAttack}]");
            
            ReduceStackCount(1);
        }
    }
}