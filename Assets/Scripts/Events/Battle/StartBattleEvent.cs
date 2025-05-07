using MyFramework.Utilities;

namespace Events.Battle
{
    public struct StartBattleEvent : IEventArgs
    {
        public int Round { get; }
        
        public StartBattleEvent(int round)
        {
            Round = round;
        }
    }
}