using System.Collections;
using Events;
using Events.Battle;
using GameLogic.Battle;
using MyFramework.Utilities;

namespace ComplexCommand
{
    public class NextTurnCommand : BaseCommand
    {
        public readonly TurnManager TurnManager;

        public NextTurnCommand(TurnManager turnManager)
        {
            // TurnManager = turnManager;
        }
        
        public override IEnumerator Execute()
        {
            // TurnManager.NextTurn();
            yield return null;
        }

        protected override IEnumerator UndoSelf()
        {
            // TurnManager.CancelNextTurn();
            yield return null;
        }
    }
}