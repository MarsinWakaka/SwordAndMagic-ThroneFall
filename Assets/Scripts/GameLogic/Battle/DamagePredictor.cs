using Events;
using GameLogic.GameAction.Attack;
using MyFramework.Utilities;

namespace GameLogic.Battle
{
    /// <summary>
    /// This class is used to predict the damage dealt by an attacker to a target
    /// </summary>
    public class DamagePredictor
    {
        public AttackAction Info;
        
        public DamagePredictor(AttackAction info)
        {
            Info = info;
        }

        // public DamageResult Calculate()
        // {
        //     // TODO 替换到BUFF系统里
        //     // EventBus.Channel(Channel.Battle).Publish<DamageContext>(GameEvent.OnPreAttack, Context);
        //     Context.Attacker.OnPreAttack?.Invoke(Context);
        //     
        //     // EventBus.Channel(Channel.Battle).Publish<DamageContext>(GameEvent.OnPreHit, Context);
        //     Context.Target.OnPreTakeDamage?.Invoke(Context);
        //
        //     var result = new DamageResult()
        //     {
        //         AttackerRT = Context.Attacker,
        //         TargetRT = Context.Target,
        //     };
        //     return result;
        // }
    }
}