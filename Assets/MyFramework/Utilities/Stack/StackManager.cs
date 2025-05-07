using System.Collections.Generic;

namespace MyFramework.Utilities.Stack
{
    public interface IStackNodeParams { }
    
    public interface IStackState
    {
        void OnEnter(IStackNodeParams parameters = null);
        void OnResume();
        void OnPause();
        void OnExit();
    }
    
    public class StackManager
    {
        private readonly Stack<IStackState> _stack = new();
        
        public void Push(IStackState stackState, IStackNodeParams parameters = null)
        {
            if (stackState == null) return;
            
            if (_stack.Count > 0)
            {
                _stack.Peek().OnPause();
            }
            
            stackState.OnEnter(parameters);
            _stack.Push(stackState);
        }
        
        public void Pop()
        {
            if (_stack.Count == 0) return;
            _stack.Pop().OnExit();
            if (_stack.Count > 0) _stack.Peek().OnResume();
        }
    }
    
    public interface IUpdatableStackState : IStackState
    {
        void OnUpdate();
    }
    
    public class UpdatableStackManager
    {
        private readonly Stack<IUpdatableStackState> _stack = new();
        
        public void Push(IUpdatableStackState stackState, IStackNodeParams parameters = null)
        {
            if (stackState == null) return;
            if (_stack.Count > 0) _stack.Peek().OnPause();
            
            stackState.OnEnter(parameters);
            _stack.Push(stackState);
        }
        
        public void Pop()
        {
            if (_stack.Count == 0) return;
            _stack.Pop().OnExit();
            if (_stack.Count > 0) _stack.Peek().OnResume();
        }

        public void Update()
        {
            if (_stack.Count > 0) _stack.Peek().OnUpdate();
        }
    }
}