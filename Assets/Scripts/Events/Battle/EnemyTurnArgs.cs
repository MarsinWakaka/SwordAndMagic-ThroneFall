using MyFramework.Utilities;

namespace Events.Battle
{
    public class EnemyTurnArgs : IEventArgs
    {
        public int CurrentTurn { get; }
        public System.Action OnComplete { get; }

        public EnemyTurnArgs(int currentTurn, System.Action onComplete)
        {
            CurrentTurn = currentTurn;
            OnComplete = onComplete;
        }
    }
}