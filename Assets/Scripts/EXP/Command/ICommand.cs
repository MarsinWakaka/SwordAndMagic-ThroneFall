using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Command
{
    public interface ICommand
    {
        public IEnumerator Execute();
        public IEnumerator Undo();
    }
}