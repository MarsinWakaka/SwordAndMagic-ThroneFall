using System;
using System.Diagnostics.CodeAnalysis;
using MyFramework.Utilities;

namespace Events.Battle
{
    public class TriggerSceneEventArgs : IEventArgs
    {
        public readonly int NewTurn;
        public readonly Action OnComplete;
        
        public TriggerSceneEventArgs(int newTurn, [NotNull] Action onComplete)
        {
            NewTurn = newTurn;
            OnComplete = onComplete;
        }
    }
}