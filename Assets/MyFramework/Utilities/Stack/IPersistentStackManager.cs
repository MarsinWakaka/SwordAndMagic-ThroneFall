using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

namespace MyFramework.Utilities.Stack
{
    public interface IPersistentStackNode
    {
        void OnEnter(IStackNodeParams parameters = null);
        void OnResume();
        void OnPause();
        void OnExit();
        void OnUpdate();
    }
    
    public class PersistentStackManager<T>
    {
        private readonly Dictionary<T, IPersistentStackNode> _registeredStackNodes = new();
        
        private readonly Stack<KeyValuePair<T, IPersistentStackNode>> _stack = new();
        private readonly HashSet<T> _keysInStack = new();
        
        public void RegisterStackNode(T key, IPersistentStackNode stackNode)
        {
            if (stackNode == null) return;
            _registeredStackNodes[key] = stackNode;
        }
        
        public void UnregisterStackNode(T key)
        {
            if (_registeredStackNodes.ContainsKey(key))
            {
                _registeredStackNodes.Remove(key);
            }
        }

        #region 原子操作

        public void Push(T key, IStackNodeParams parameters = null)
        {
            if (!_registeredStackNodes.TryGetValue(key, out var stackNode))
            {
                Debug.LogError($"PersistentStackManager: No stack node registered for key {key}");
                return;
            }
            if (_keysInStack.Contains(key))
            {
                // 保持弹出后的顶部节点的暂停状态
                PopUntil(key, true, false);
            }
            else
            {
                // 如果栈不为空，需要暂停栈顶节点
                if (_stack.Count > 0) _stack.Peek().Value.OnPause();
            }
            _stack.Push(new KeyValuePair<T, IPersistentStackNode>(key, stackNode));
            _keysInStack.Add(key);
            stackNode.OnEnter(parameters);
            stackNode.OnResume();
        }
        
        public void Pop(bool resumeTop = true)
        {
            if (_stack.Count == 0) return;
            var top = _stack.Pop();
            top.Value.OnPause();
            top.Value.OnExit();
            _keysInStack.Remove(top.Key);
            if (resumeTop && _stack.Count > 0) _stack.Peek().Value.OnResume();
        }

        #endregion
        
        private void PopUntil(T key, bool includeKey = false, bool resumeTop = true)
        {
            while (_stack.Count > 0)
            {
                var top = _stack.Peek();
                if (top.Key.Equals(key))
                {
                    if (includeKey) Pop(resumeTop);
                    break;
                }
                Pop(false);
            }
        }
        
        public void PopAll()
        {
            while (_stack.Count > 0)
            {
                Pop(false);
            }
        }
        
        public void OnUpdateTop()
        {
            if (_stack.Count > 0) _stack.Peek().Value.OnUpdate();
        }
    }
}