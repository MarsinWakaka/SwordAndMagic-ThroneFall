using System;
using UnityEngine;
using UnityEngine.Events;

namespace MyFramework.Utilities
{
    [Serializable]
    public class Bindable<T>
    {
        [SerializeField] private T value;
        public T Value
        {
            get => value;
            set
            {
                if (Equals(this.value, value)) return;
                this.value = value;
                _onValueChanged?.Invoke(this.value);
            }
        }
        
        // 不序列化（因为暂时没有这个需求）
        private UnityEvent<T> _onValueChanged;
        
        public void AddListener(UnityAction<T> action)
        {
            _onValueChanged ??= new UnityEvent<T>();
            _onValueChanged.AddListener(action);
        }
        
        public void RemoveListener(UnityAction<T> action)
        {
            _onValueChanged?.RemoveListener(action);
        }

        public Bindable(T value = default)
        {
            this.value = value;
        }
    }
}