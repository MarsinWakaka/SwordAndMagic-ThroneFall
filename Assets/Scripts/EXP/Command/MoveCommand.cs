using System.Collections;
using System.Linq;
using GameLogic.Grid.PathFinding;
using GameLogic.Unit;
using GameLogic.Unit.Controller;

namespace Command
{
    public class MoveCommand : ICommand
    {
        public readonly CharacterUnitController CharacterToMove;
        public readonly MovePath Path;
        
        public MoveCommand(CharacterUnitController characterToMove, MovePath path)
        {
            CharacterToMove = characterToMove;
            Path = path;
        }
        
        public IEnumerator Execute()
        {
            var pathNodes = Path.PathNode;
            // var triggerService = ServiceLocator.GetService<ITriggerService>();
            foreach (var node in pathNodes.TakeWhile(node => !CharacterToMove.IsDead))
            {
                // yield return CharacterToMove.Move(node);
            }
            yield return null;
        }

        public IEnumerator Undo()
        {
            yield return null;
        }
    }
}