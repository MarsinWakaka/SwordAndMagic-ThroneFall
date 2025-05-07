using System;
using GameLogic.Grid.Area;
using MyFramework.Utilities;

namespace Events.Battle
{
    public class CalculateAttackableAreaRequest : IEventArgs
    {
        public readonly AttackParam AttackParams;
        public readonly Action<AttackableAreaResult> OnCalculateAttackableAreaCompleted;
        
        public CalculateAttackableAreaRequest(AttackParam attackParams, Action<AttackableAreaResult> onCalculateAttackableAreaCompleted)
        {
            AttackParams = attackParams;
            OnCalculateAttackableAreaCompleted = onCalculateAttackableAreaCompleted;
        }
    }
}