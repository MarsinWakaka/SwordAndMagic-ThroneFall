using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine.SceneManagement;

#region 核心数据结构
public enum UILayer
{
    Background = 1000,
    Normal = 2000,
    Popup = 3000,
    System = 4000
}

public enum UIState
{
    Active,
    Paused,
    Hidden,
    Destroyed
}

[System.Serializable]
public class UIStateSnapshot
{
    public string panelKey;
    public UILayer layer;
    public byte[] stateData;
    public int sortingOrder;
    public List<CoroutineInfo> runningCoroutines = new List<CoroutineInfo>();
}

public struct CoroutineInfo
{
    public string methodName;
    public object[] parameters;
}
#endregion

#region UI面板基类
public abstract class UIBasePanel : MonoBehaviour
{
    protected UILayer _layer = UILayer.Normal;
    [SerializeField] private bool _persistentAcrossScenes = false;
    
    private Canvas _canvas;
    private GraphicRaycaster _raycaster;
    private readonly Dictionary<string, object> _runtimeState = new Dictionary<string, object>();
    private readonly List<Coroutine> _activeCoroutines = new List<Coroutine>();
    
    public string PanelKey { get; private set; }
    public UILayer Layer => _layer;
    public UIState CurrentState { get; protected set; } = UIState.Destroyed;
    public bool IsPersistent => _persistentAcrossScenes;

    protected virtual void Awake()
    {
        InitializeComponents();
        RegisterPanel();
    }

    private void InitializeComponents()
    {
        _canvas = gameObject.AddComponent<Canvas>();
        _raycaster = gameObject.AddComponent<GraphicRaycaster>();
        
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingLayerName = "UI";
        _canvas.sortingOrder = (int)_layer;
    }

    private void RegisterPanel()
    {
        PanelKey = GetType().Name;
        UIManagerDemo.Instance.RegisterPanel(this);
    }

    public virtual void Initialize(object data = null)
    {
        CurrentState = UIState.Active;
    }

    public virtual void Open(object data = null)
    {
        gameObject.SetActive(true);
        CurrentState = UIState.Active;
        UpdateSortingOrder();
        
        if (data != null)
        {
            // RestoreTransientState(data);
        }
    }

    public virtual void Close()
    {
        CurrentState = UIState.Hidden;
        gameObject.SetActive(false);
        UIManagerDemo.Instance.ReturnToPool(this);
    }

    public virtual void Pause()
    {
        if (CurrentState != UIState.Active) return;
        
        CurrentState = UIState.Paused;
        SaveStateSnapshot();
        SetInteractivity(false);
        SuspendAnimations();
        SuspendCoroutines();
    }

    public virtual void Resume()
    {
        if (CurrentState != UIState.Paused) return;
        
        CurrentState = UIState.Active;
        RestoreStateSnapshot();
        SetInteractivity(true);
        ResumeAnimations();
        ResumeCoroutines();
    }

    protected new Coroutine StartCoroutine(IEnumerator routine)
    {
        var coroutine = base.StartCoroutine(routine);
        _activeCoroutines.Add(coroutine);
        return coroutine;
    }

    private void SaveStateSnapshot()
    {
        // _runtimeState["ScrollPos"] = GetScrollPosition();
        // _runtimeState["InputText"] = GetInputText();
        // 添加其他需要保存的状态...
    }

    private void RestoreStateSnapshot()
    {
        // SetScrollPosition((Vector2)_runtimeState["ScrollPos"]);
        // SetInputText((string)_runtimeState["InputText"]);
        // 恢复其他状态...
    }

    private void SetInteractivity(bool state)
    {
        _raycaster.enabled = state;
        foreach (var selectable in GetComponentsInChildren<Selectable>())
        {
            selectable.interactable = state;
        }
    }

    private void SuspendAnimations()
    {
        foreach (var anim in GetComponentsInChildren<Animator>())
        {
            anim.speed = 0;
        }
    }

    private void ResumeAnimations()
    {
        foreach (var anim in GetComponentsInChildren<Animator>())
        {
            anim.speed = 1;
        }
    }

    private void SuspendCoroutines()
    {
        foreach (var coroutine in _activeCoroutines)
        {
            StopCoroutine(coroutine);
        }
    }

    private void ResumeCoroutines()
    {
        // foreach (var info in _persistedCoroutines)
        // {
        //     var method = GetType().GetMethod(info.methodName);
        //     if (method != null)
        //     {
        //         StartCoroutine((IEnumerator)method.Invoke(this, info.parameters));
        //     }
        // }
        // _persistedCoroutines.Clear();
    }

    private void UpdateSortingOrder()
    {
        _canvas.sortingOrder = UIManagerDemo.Instance.GetNextSortingOrder(_layer);
    }

    protected virtual void OnDestroy()
    {
        CurrentState = UIState.Destroyed;
        UIManagerDemo.Instance.UnregisterPanel(this);
    }
}
#endregion

#region UI管理器
public class UIManagerDemo : MonoBehaviour
{
    #region Singleton
    private static UIManagerDemo _instance;
    private static readonly object _lock = new object();
    private static bool _isQuitting = false;

    public static UIManagerDemo Instance
    {
        get
        {
            if (_isQuitting) return null;

            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<UIManagerDemo>();
                    if (_instance == null)
                    {
                        var go = new GameObject("UIManager");
                        _instance = go.AddComponent<UIManagerDemo>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }
    }
    #endregion

    private Dictionary<UILayer, int> _layerCounters = new Dictionary<UILayer, int>();
    private Dictionary<string, Stack<UIBasePanel>> _panelPool = new Dictionary<string, Stack<UIBasePanel>>();
    private Dictionary<string, UIBasePanel> _activePanels = new Dictionary<string, UIBasePanel>();
    private Dictionary<UILayer, List<UIBasePanel>> _pausedPanels = new Dictionary<UILayer, List<UIBasePanel>>();

    private void Awake()
    {
        InitializeLayerSystem();
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void InitializeLayerSystem()
    {
        foreach (UILayer layer in System.Enum.GetValues(typeof(UILayer)))
        {
            _layerCounters[layer] = (int)layer;
            _pausedPanels[layer] = new List<UIBasePanel>();
        }
    }

    public int GetNextSortingOrder(UILayer layer)
    {
        _layerCounters[layer] += 10;
        return _layerCounters[layer];
    }

    public void RegisterPanel(UIBasePanel panel)
    {
        if (!_activePanels.ContainsKey(panel.PanelKey))
        {
            _activePanels.Add(panel.PanelKey, panel);
        }
    }

    public void UnregisterPanel(UIBasePanel panel)
    {
        _activePanels.Remove(panel.PanelKey);
    }

    public void OpenPanel<T>(object data = null) where T : UIBasePanel
    {
        StartCoroutine(OpenPanelAsync<T>(data));
    }

    private IEnumerator OpenPanelAsync<T>(object data) where T : UIBasePanel
    {
        string panelKey = typeof(T).Name;
        
        if (TryGetFromPool(panelKey, out UIBasePanel panel))
        {
            // ActivatePanel(panel, data);
            yield break;
        }

        var handle = Addressables.LoadAssetAsync<GameObject>(panelKey);
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var instance = Instantiate(handle.Result);
            panel = instance.GetComponent<T>();
            
            if (panel != null)
            {
                // InitializePanel(panel, data);
                if (panel.IsPersistent)
                {
                    DontDestroyOnLoad(instance);
                }
            }
        }
    }

    private bool TryGetFromPool(string panelKey, out UIBasePanel panel)
    {
        if (_panelPool.TryGetValue(panelKey, out Stack<UIBasePanel> stack) && stack.Count > 0)
        {
            panel = stack.Pop();
            return true;
        }
        panel = null;
        return false;
    }

    public void ReturnToPool(UIBasePanel panel)
    {
        string key = panel.PanelKey;
        if (!_panelPool.ContainsKey(key))
        {
            _panelPool[key] = new Stack<UIBasePanel>();
        }
        panel.gameObject.SetActive(false);
        _panelPool[key].Push(panel);
    }

    public void PauseLayer(UILayer layer)
    {
        foreach (var panel in _activePanels.Values.Where(p => p.Layer == layer))
        {
            panel.Pause();
            _pausedPanels[layer].Add(panel);
        }
    }

    public void ResumeLayer(UILayer layer)
    {
        foreach (var panel in _pausedPanels[layer])
        {
            panel.Resume();
        }
        _pausedPanels[layer].Clear();
    }

    private void OnSceneUnloaded(UnityEngine.SceneManagement.Scene scene)
    {
        foreach (var panel in _activePanels.Values.ToList())
        {
            if (!panel.IsPersistent)
            {
                panel.Close();
                Addressables.ReleaseInstance(panel.gameObject);
            }
        }
    }

    private void OnApplicationQuit()
    {
        _isQuitting = true;
        CleanupResources();
    }

    private void CleanupResources()
    {
        foreach (var pool in _panelPool.Values)
        {
            while (pool.Count > 0)
            {
                var panel = pool.Pop();
                Addressables.ReleaseInstance(panel.gameObject);
            }
        }
    }
}
#endregion

#region 使用示例
public class SettingsPanel : UIBasePanel
{
    [SerializeField] private ScrollRect _settingsScroll;
    [SerializeField] private InputField _searchInput;

    protected override void Awake()
    {
        base.Awake();
        _layer = UILayer.Popup;
    }

    public override void Initialize(object data = null)
    {
        // 初始化设置项
    }

    private Vector2 GetScrollPosition() => _settingsScroll.normalizedPosition;
    private void SetScrollPosition(Vector2 pos) => _settingsScroll.normalizedPosition = pos;

    private string GetInputText() => _searchInput.text;
    private void SetInputText(string text) => _searchInput.text = text;

    public override void Pause()
    {
        base.Pause();
        // 保存额外状态
    }

    public override void Resume()
    {
        base.Resume();
        // 恢复额外状态
    }
}
#endregion