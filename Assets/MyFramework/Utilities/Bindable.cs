using System;
using UnityEngine;
using UnityEngine.Events;

namespace MyFramework.Utilities
{
    // [Serializable]
    /// <summary>
    /// 不可序列化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Bindable<T>
    {
        [SerializeField] 
        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                if (Equals(this._value, value)) return;
                this._value = value;
                OnValueChanged?.Invoke(this._value);
            }
        }
        
        // 不序列化（因为暂时没有这个需求）
        private event Action<T> OnValueChanged;
        
        public void AddListener(Action<T> action)
        {
            OnValueChanged += action;
        }
        
        public void RemoveListener(Action<T> action)
        {
            OnValueChanged -= action;
        }

        public Bindable(T value = default)
        {
            this._value = value;
        }
    }
}