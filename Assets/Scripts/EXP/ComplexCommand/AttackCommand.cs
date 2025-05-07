using System.Collections;
using GameLogic.Unit;
using GameLogic.Unit.Controller;

namespace ComplexCommand
{
    public class AttackCommand : BaseCommand
    {
        public readonly EntityController Controller;

        public AttackCommand(EntityController controller)
        {
            Controller = controller;
        }
        
        public override IEnumerator Execute()
        {
            yield return null;
        }

        protected override IEnumerator UndoSelf()
        {
            yield return null;
        }
    }
}