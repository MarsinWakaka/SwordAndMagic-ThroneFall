using Core.Log;
using GameLogic.BUFF;
using GameLogic.Unit;
using GameLogic.Unit.Controller;

namespace GameLogic.GameAction.Attack
{
    public enum ActionType
    {
        // 主动
        Active,
        // 反应
        Reactive 
    }
    
    // public class Damage
    
    public class AttackAction
    {
        public readonly CharacterUnitController Attacker;
        public readonly CharacterUnitController Defender;
        public readonly ActionType ActionType;
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public DamageSegment[] DamageSegments;  // 可能会有附加伤害BUFF

        private Faction AttackerFaction => Attacker.RuntimeData.faction;
        
        public AttackAction(
            CharacterUnitController attacker, 
            CharacterUnitController defender, 
            ActionType actionType, 
            DamageSegment[] damageSegments)
        {
            Attacker = attacker;
            Defender = defender;
            ActionType = actionType;
            DamageSegments = damageSegments;
        }
        
        public void Execute()
        {
            var attackerFactionTriggers = BuffSystem.Instance.GetFactionBuffs(AttackerFaction);
            var attackerHasTrigger = attackerFactionTriggers.LocalBuffs.TryGetValue(Attacker.RuntimeData.InstanceID, out var attackerTriggerSet);
            
            // if (Target is not CharacterUnitController defender)
            // {
            //     // TODO 处理非角色单位的攻击
            //     attackerTriggerSet?.TriggerPreDoDamage(this);
            //     foreach (var attackSegment in AttackSegments) Target.TakeDamage(attackSegment);
            //     return;
            // }
            
            var defenderFaction = Defender.CharacterRuntimeData.faction;
            var defenderFactionTriggers = BuffSystem.Instance.GetFactionBuffs(defenderFaction);
            var defenderHasTrigger = defenderFactionTriggers.LocalBuffs.TryGetValue(Defender.RuntimeData.InstanceID, out var defenderTriggerSet);
            var isSameFaction = AttackerFaction == defenderFaction;
            
            // [预先反应]
            // 防御方
            defenderFactionTriggers.FactionBuffTriggers.TriggerPreTakeDamage(this);// 防御反击
            if (defenderHasTrigger) defenderTriggerSet.TriggerPreTakeDamage(this);// 减伤BUFF，受伤前回血BUFF
            
            // 进攻方
            if (attackerHasTrigger) attackerTriggerSet.TriggerPreDoDamage(this);// 预先支援攻击
            if (!isSameFaction)
            {
                attackerFactionTriggers.FactionBuffTriggers.TriggerPreDoDamage(this);// 预先增伤BUFF
            }

            foreach (var damageSegment in DamageSegments)
            {
                BattleLogManager.Instance.Log(
                    $"{Attacker.FriendlyInstanceID()} 对 " +
                    $"{Defender.FriendlyInstanceID()} 造成了 " +
                    $"{damageSegment.Damage}点{damageSegment.DamageType}伤害");
                Defender.TakeDamage(damageSegment);// 受击
            }
            
            // [后续反应]
            // 防御方
            defenderFactionTriggers.FactionBuffTriggers.TriggerAfterTakeDamage(this);// ？
            if (defenderHasTrigger) defenderTriggerSet.TriggerAfterTakeDamage(this);// ？
            
            // 进攻方
            if (attackerHasTrigger) attackerTriggerSet.TriggerAfterDoDamage(this);// 攻击后为队友回血BUFF | 吸血BUFF(需要攻击结果上下文)
            if (!isSameFaction)
            {
                attackerFactionTriggers.FactionBuffTriggers.TriggerAfterDoDamage(this);// 攻击后支援攻击
            }
        }
    }
}