using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MyFramework.Utilities.Singleton;
using UnityEngine;
using UnityEngine.Assertions;

namespace UI
{
    public class UIManager : ThreadSafeMonoSingleton<UIManager>
    {
        public class PanelRecord
        {
            /// <summary>
            /// 面板名称
            /// </summary>
            public string PanelName;
        
            /// <summary>
            /// 面板打开时传入的数据
            /// </summary>
            public object Data;

            /// <summary>
            /// 面板缓存[可选]
            /// </summary>
            [CanBeNull]
            public BaseUIPanel PanelCache;

            public PanelRecord Initialize(string panelName, object onOpenData, BaseUIPanel panelCache = null)
            {
                PanelName = panelName;
                Data = onOpenData;
                PanelCache = panelCache;
                return this;
            }
        }

        [SerializeField] private Transform panelRoot;
        [SerializeField] private UIManagerConfig uiManagerConfig;
        
        private readonly Dictionary<string, string> _panelPathDict = new();
        private readonly List<PanelRecord> _panelStack = new();
        private BaseUIPanel _curPanel;

        protected override void WhenAwake()
        {
            base.WhenAwake();
            // 扫描PanelName，将其全部添加值_panelPathDict
            var names = PanelName.GetAllPanelNames();
            foreach (var panelName in names)
            {
                _panelPathDict.Add(panelName, GetFullPath(panelName));
            }
        }

        private string GetFullPath(string panelName) => uiManagerConfig.panelPathRootPath + panelName;

        /// <summary>
        /// 首先弹出栈顶面板，根据策略决定是否隐藏或销毁它
        /// 再检查是否有同名面板在栈中，
        ///     如果有，则关闭它以及它之后的面板（但希望能够复用它）[会导致第一步可能被缓存的面板丢失]
        ///     如果没有，则创建一个新的面板实例
        /// 最后打开面板
        /// </summary>
        public void ShowPanel(string panelName, OpenStrategy strategy, object data = null)
        {
            if (_panelStack.Count > 0)
            {
                // var topRecord = _panelStack[^1];
                // topRecord.PanelCache = _curPanel;
                switch (strategy)
                {
                    case OpenStrategy.Additive:
                        // 叠加模式
                        break;
                    case OpenStrategy.PauseCurrent:
                        _curPanel.OnPause();
                        break;
                    case OpenStrategy.HideCurrent:
                        _curPanel.OnHide();
                        break;
                    case OpenStrategy.ReplaceCurrent:
                        _curPanel.OnRelease();
                        _panelStack.RemoveAt(_panelStack.Count - 1);
                        break;
                    case OpenStrategy.CloseAllFirst:
                        // 清理模式
                        ClearAllPanel();
                        break;
                }
            }
            
            // 关闭与当前面板同名的面板以及之后的面板
            DestroyPanelsAfter(panelName);
            
            if (_panelStack.Count > 0 && _panelStack[^1].PanelName == panelName)
            {
                _panelStack[^1].Data = data;
                RecoverTopPanelState();
            }
            else
            {
                _curPanel = GetPanel(panelName);
                _curPanel.OnCreate(data);
                _panelStack.Add(new PanelRecord().Initialize(panelName, data, _curPanel));
            }
            Debug.Log("打开面板: " + panelName + "\t面板栈长度: " + _panelStack.Count);
        }
        
        /// <summary>
        /// 首先关闭与打开面板同名的面板以及之后的面板
        /// 然后启用关闭后栈顶的面板
        /// </summary>
        public void ClosePanel(string panelName)
        {
            if (!DoesPanelOpened(panelName))
            {
                Debug.LogWarning($"面板未打开: {panelName}");
                return;
            }
            
            // 关闭栈上指定面板之上的面板
            DestroyPanelsAfter(panelName);
            
            // 关闭指定面板
            Assert.IsTrue(_panelStack[^1].PanelName == panelName);
            var topPanelRecord = _panelStack[^1];
            topPanelRecord.PanelCache?.OnRelease();
            _panelStack.RemoveAt(_panelStack.Count - 1);
            
            // 恢复栈顶面板状态
            if (_panelStack.Count > 0) RecoverTopPanelState();
            Debug.Log("关闭面板: " + panelName + "\t面板栈长度: " + _panelStack.Count);
        }

        public void ClearAllPanel()
        {
            // 关闭所有面板
            foreach (var panelRecord in _panelStack)
            {
                panelRecord.PanelCache?.OnRelease();
            }
            _panelStack.Clear();
        }

        private void DestroyPanelsAfter(string panelName)
        {
            var startIndex = _panelStack.FindIndex(panel => panel.PanelName == panelName);
            if (startIndex != -1)
            {
                // 这里不要使用迭代器，因为在迭代器中删除元素会导致异常
                while (_panelStack.Count - 1 > startIndex)
                {
                    var panelRecord = _panelStack[^1];   // 获取栈顶面板
                    panelRecord.PanelCache?.OnRelease();
                    _panelStack.RemoveAt(_panelStack.Count - 1);
                }
            }
        }
        
        /// <summary>
        /// 恢复栈顶面板状态
        /// </summary>
        private void RecoverTopPanelState()
        {
            if (_panelStack.Count == 0)
            {
                Debug.LogError("面板栈为空，无法恢复栈顶面板状态");
                return;
            }
            
            var topPanelRecord = _panelStack[^1];
            var topPanel = topPanelRecord.PanelCache;
            var data = topPanelRecord.Data;
            if (topPanel != null) {
                switch (topPanel.PanelState)
                {
                    case PanelState.Active:
                        // 面板处于激活状态，直接返回
                        break;
                    case PanelState.Hidden:
                        topPanel.OnShow(data);
                        break;
                    case PanelState.Paused:
                        topPanel.OnResume(data);
                        break;
                    case PanelState.Uninitialized:
                        topPanel.OnCreate(data);
                        break;
                    default:
                        Debug.LogError($"面板状态异常: {topPanel.PanelState}");
                        break;
                }
            }
            else
            {
                // 如果没有缓存面板，则重新加载面板
                var panel = GetPanel(topPanelRecord.PanelName);
                if (panel != null)
                {
                    panel.OnCreate(topPanelRecord.Data);
                    topPanelRecord.PanelCache = panel;
                }
            }
            _curPanel = topPanel;
        }
        
        private bool DoesPanelOpened(string panelName)
        {
            return _panelStack.FindIndex(panel => panel.PanelName == panelName) != -1;
        }

        private BaseUIPanel GetPanel(string panelName)
        {
            // TODO 检测缓存，如果有直接从缓存加载。
            
            var panelPath = _panelPathDict[panelName];
            if (panelPath == null) 
            {
                Debug.LogError($"未知面板路径: {panelName}");
                return null;
            }
            
            return LoadPanel(panelName);
        }

        private BaseUIPanel LoadPanel(string panelName)
        {
            var panelPath = _panelPathDict[panelName];
            var panelPrefab = Resources.Load<GameObject>(panelPath);
            if (panelPrefab != null)
            {
                var panel = Instantiate(panelPrefab, panelRoot).GetComponent<BaseUIPanel>();
                if (panel == null)
                {
                    Debug.LogError($"面板预制体未包含BaseUIPanel组件: {panelPath}");
                    return null;
                }
                return panel;
            }
            Debug.LogError($"面板预制体未找到: {panelPath}");
            return null;
        }
    }
}