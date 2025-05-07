using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ComplexCommand
{
    public abstract class BaseCommand
    {
        public List<BaseCommand> SubCommands { get; protected set; }
        
        public void AddSubCommand(BaseCommand command)
        {
            SubCommands ??= new List<BaseCommand>();
            SubCommands.Add(command);
        }
        
        public abstract IEnumerator Execute();

        public IEnumerator Undo()
        {
            if (SubCommands is { Count: > 0 })
            {
                foreach (var subCommand in SubCommands.TakeWhile(subCommand => subCommand != null))
                {
                    yield return subCommand.Undo();
                }
            }
            UndoSelf();
        }

        protected abstract IEnumerator UndoSelf();
    }
}