using System;
using UnityEngine;

namespace MyFramework.Utilities.Singleton
{
    /// <summary>
    /// 增强型Mono单例基类（自动跨场景/线程安全/资源释放）
    /// </summary>
    public class ThreadSafeMonoSingleton<T> : MonoBehaviour where T : ThreadSafeMonoSingleton<T>
    {
        #region 核心字段与属性
        private static readonly object Lock = new();
        private static volatile T _instance; // 添加volatile保证可见性
        private static bool _isQuitting;

        /// <summary>
        /// 安全访问的全局实例
        /// </summary>
        public static T Instance
        {
            get
            {
                if (_isQuitting) {
                    Debug.LogWarning($"[Singleton] Instance '{typeof(T)}' already destroyed.");
                    return null;
                }

                // 第一重无锁检查
                if (_instance != null) return _instance;
                lock (Lock)
                {
                    // 第二重加锁检查
                    if (_instance == null)
                    {
                        _instance = FindExistingInstance() ?? CreateNewInstance();
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region 实例管理逻辑（保持原有逻辑不变）

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = FindExistingInstance() ?? CreateNewInstance();
                InitializeSingleton();
            }
            else
            {
                Destroy(this);
            }
        }

        private static T FindExistingInstance()
        {
            T[] instances = FindObjectsOfType<T>(true);

            if (instances.Length > 1) {
                Debug.LogError($"[Singleton] 发现多个{typeof(T)}实例! 清理冗余实例...");
                for (int i = 1; i < instances.Length; i++) {
                    Destroy(instances[i].gameObject);
                }
            }

            return instances.Length > 0 ? instances[0] : null;
        }

        private static T CreateNewInstance()
        {
            if (!Application.isPlaying) {
                Debug.LogWarning($"[Singleton] 尝试在非播放模式创建{typeof(T)}实例");
                return null;
            }

            var obj = new GameObject($"{typeof(T).Name}_Singleton");
            return obj.AddComponent<T>();
        }

        #endregion

        #region 生命周期管理
        protected virtual void InitializeSingleton()
        {
            Debug.Log($"[Singleton] 初始化 {typeof(T)} 实例");
            DontDestroyOnLoad(gameObject);
            WhenAwake();
        }

        protected virtual void OnApplicationQuit()
        {
            _isQuitting = true;
            WhenApplicationQuit();
        }

        protected virtual void OnEnable() 
        {
            if (_instance == this)
            {
                WhenEnable();
            }
        }

        protected virtual void OnDisable()
        {
            if (_instance == this)
            {
                WhenDisable();
            }
        }
        
        protected virtual void OnDestroy()
        {
            if (_instance == this) {
                _instance = null;
                WhenDestroy();
            }
        }
        #endregion

        #region 可扩展的虚方法
        /// <summary>
        /// 首次初始化时调用（替代Awake）
        /// </summary>
        protected virtual void WhenAwake() { }

        protected virtual void WhenEnable() { }
        
        protected virtual void WhenDisable() { }

        /// <summary>
        /// 应用程序退出前调用
        /// </summary>
        protected virtual void WhenApplicationQuit() { }

        /// <summary>
        /// 实例销毁时调用（替代OnDestroy）
        /// </summary>
        protected virtual void WhenDestroy() { }
        #endregion
    }
}