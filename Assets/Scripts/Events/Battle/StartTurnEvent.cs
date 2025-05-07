using System;
using GameLogic.Unit;
using MyFramework.Utilities;

namespace Events.Battle
{
    
    public class StartTurnEvent : IEventArgs
    {
        public int Round;
        public readonly Faction Faction;
        /// <summary>
        /// 玩家回合结束回调
        /// </summary>
        private Action _onComplete;
        public StartTurnEvent(int round, Faction faction, Action onComplete)
        {
            Round = round;
            Faction = faction;
            _onComplete = onComplete;
        }
        
        public void Complete()
        {
            _onComplete?.Invoke();
            _onComplete = null;
        }
    }
}