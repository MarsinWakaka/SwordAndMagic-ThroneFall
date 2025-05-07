using System;

namespace MyFramework.Utilities
{
    /// <summary>
    /// 限定T为值类型
    /// </summary>
    public class BindableD<T> where T : struct
    {
        private T _value;
        public T Value
        {
            get => _value;
            set
            {
                if (Equals(_value, value)) return;
                var oldValue = _value;
                _value = value;
                OnValueChanged?.Invoke(oldValue, _value);
            }
        }
        
        /// <summary>
        /// 变化时，返回旧数据和新数据
        /// </summary>
        public Action<T, T> OnValueChanged;

        public BindableD(T value = default)
        {
            _value = value;
        }
    }
}