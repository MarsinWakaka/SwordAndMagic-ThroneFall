using System.Collections;
using GameLogic.Grid.PathFinding;
using GameLogic.Unit;
using GameLogic.Unit.Controller;

namespace ComplexCommand
{
    public class MoveCommand : BaseCommand
    {
        public readonly CharacterUnitController CharacterToMove;
        public readonly MovePath Path;
        
        // private int _actualMoveCost; // 对于那些需要消耗移动点来移动的就可以用这个来记录消耗的移动点数
        
        public MoveCommand(CharacterUnitController characterToMove, MovePath path)
        {
            CharacterToMove = characterToMove;
            Path = path;
        }

        public override IEnumerator Execute()
        {
            var pathNodes = Path.PathNode;
            // var triggerService = ServiceLocator.GetService<ITriggerService>();
            foreach (var node in pathNodes)
            {
                if (CharacterToMove.IsDead) break;
                // yield return CharacterToMove.Move(node);
            }
            yield return null;
        }

        protected override IEnumerator UndoSelf()
        {
            yield return null;
        }
    }
}