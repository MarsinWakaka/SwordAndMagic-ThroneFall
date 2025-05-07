using System.Collections.Generic;
using UnityEngine;

namespace MyFramework.Utilities.FSM
{
    public class Fsm<T> 
    {
        private readonly Dictionary<T, IState> _states = new();
        private T _curState;
        public IState CurrentState => _states.GetValueOrDefault(_curState);
        
        public Fsm(T initialState)
        {
            _curState = initialState;
            _states.Add(_curState, new EmptyState());
        }
        
        public void AddState(T stateName, IState state)
        {
            if (_states.TryAdd(stateName, state)) return;
            _states[stateName] = state;
            Debug.LogWarning($"State {stateName} already exists in FSM. Overwriting the existing state.");
        }
        
        public void RemoveState(T stateName)
        {
            if (_states.ContainsKey(stateName))
            {
                _states.Remove(stateName);
            }
            else
            {
                Debug.LogError($"State {stateName} not found in FSM.");
            }
        }
        
        public void ChangeState(T stateName, 
            IStateParams newStateEnterParams = null, 
            IStateParams oldStateExitParams = null)
        {
            if (_states.TryGetValue(stateName, out var state))
            {
                _states[_curState].OnExit(oldStateExitParams);
                _curState = stateName;
                _states[_curState].OnEnter(newStateEnterParams);
            }
            else
            {
                Debug.LogError($"State {stateName} not found in FSM.");
            }
        }
        
        public void UpdateFsm()
        {
            _states[_curState].OnUpdate();
        }
    }
}