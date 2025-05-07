// - 分为模态窗口与非模态窗口
// 模态窗口：在打开模态窗口时，不能与其他窗口交互
// 非模态窗口：可以与其他窗口交互

// - 支持不同的打开方式
// AdditiveWithDoNothing：[例如打开综合界面]
// AdditiveWithRestoreCurrent：[打开大多数的窗口的方式]
// 在当前窗口上添加一个新的窗口，存储当前窗口的数据并关闭，然后打开新的窗口
// AdditiveWithPauseCurrent：[例如打开聊天窗口、综合面板的按钮]
// 在当前窗口上添加一个新的窗口，暂停当前窗口的交互，当该面板退出时，需要恢复上次窗口的交互（如果有的话）
// Replace：替换当前窗口，关闭当前窗口并打开新的窗口 [登录界面的登录按钮]
// Single：关闭其它所有UI面板，打开指定的UI面板 [例如综合面板的回到主界面按钮]

// - [支持拖拽]、支持创建、打开、关闭、暂停、恢复、销毁(状态分为激活、非激活、销毁)
// 面板打开时，创建面板对象并激活，播放打开动画
// 面板暂停时，需要隔断交互（考虑使用CanvasGroup的interactable属性）、同时UI画面需要暂停
// 面板恢复时，需要恢复交互

// - 实现动静态分层
// 窗口分为静态窗口与动态窗口、将经常变化的窗口通过Canvas隔离开来、防止引起全部UI的重绘

// - UI动画实现
// 使用Tween动画库实现简单UI动画、[学习其它动画的实现方式]

// - 支持UI与粒子效果的结合

// [可选]
// 实现红点系统
// 屏幕点击特效

// 登录界面
// -Single Or Replace-> 主界面
// 主界面
//  -Replace-> 综合界面
//      -Replace-> 角色界面
//          -Additive-> 角色详细界面
//              -SubPanel-> 角色升级面板(禁用角色详细界面的交互)
//              -SubPanel-> 角色技能面板
//              -Additive-> 角色职业升阶面板

using System;
using UnityEngine;
using UnityEngine.Serialization;

public enum PanelType
{
    Modal,     // 模态窗口
    NonModal,  // 非模态窗口
}

public enum PanelAnimation
{
    None,      // 无动画
    FadeIn,    // 渐入
    FadeOut,   // 渐出
    SlideIn,   // 滑入
    SlideOut,  // 滑出
}

public enum PanelSortingOrder
{
    Low,       // 低优先级
    Medium,    // 中优先级
    High,      // 高优先级
}

public enum InitState {
    Active,     // 立即激活
    Preloaded,  // 预加载不显示
    Lazy        // 延迟加载
}

public enum OpenStrategy 
{
    // 叠加模式：不影响当前面板的动画等，禁用当前面板输入，但保持可见
    Additive,
    
    // 暂停模式：暂停当前面板的动画，禁用当前面板输入，但保持可见（适合弹窗）
    // Pause模式需要禁用当前面板的交互组件，但保持其可见状态，比如禁用按钮和射线检测。
    PauseCurrent,
    
    // 隐藏模式：隐藏当前面板，保留在内存（适合临时切换视图）
    // Pause模式需要禁用当前面板的交互组件，但保持其可见状态，比如禁用按钮和射线检测。
    HideCurrent,
    
    // 替换模式：关闭当前面板并打开新面板（适合全屏界面切换）
    ReplaceCurrent,
    
    // // 缓存模式：关闭当前面板并打开新面板，保留当前面板在内存中（适合临时切换视图）
    // CacheCurrent,
    
    // 清理模式：关闭所有面板后打开（适合返回主页）
    CloseAllFirst
}

public enum PanelCloseStrategy 
{
    // 立即销毁，释放资源
    Destroy,
    
    // 隐藏并缓存，可快速重新打开
    HideAndCache,
    
    // 标记为可销毁，由资源管理系统回收
    MarkForGC
}

public enum PanelState
{
    // 未初始化状态
    Uninitialized,
    
    // 激活状态
    Active,
    
    // 暂停状态
    Paused,
    
    // 隐藏状态
    Hidden,
    
    // 销毁状态
    Destroyed,
}

public enum AutoClose 
{
    None,            // 手动关闭
    ClickOutside,    // 点击外部关闭
    Timeout,         // 倒计时关闭
    SceneChange      // 场景切换关闭
}

// 叠加并暂停 OR 子面板
// 叠加并存储当前窗口

namespace UI
{
    public interface IRecyclable
    {
        void ResetState();
    }
    
    public abstract class BaseUIPanel : MonoBehaviour
    {
        public PanelState PanelState { get; set; } = PanelState.Uninitialized;
        
        // 请求数据，显示可见，启用输入，播放打开动画
        public virtual void OnCreate(object data)
        {
            PanelState = PanelState.Active;
        }
        
        // 释放数据，显示不可见，禁用输入
        public virtual void OnRelease()
        {
            PanelState = PanelState.Destroyed;
            // TODO 释放资源管理器
            Destroy(gameObject);
        }
        
        // 设置可见，恢复输入，并播放打开动画
        public virtual void OnShow(object data)
        {
            PanelState = PanelState.Active;
            gameObject.SetActive(true);
        }
        
        // 设置不可见，禁用输入
        public virtual void OnHide()
        {
            PanelState = PanelState.Hidden;
            gameObject.SetActive(false);
        }
        
        // 恢复UI动效，恢复输入
        public virtual void OnResume(object data)
        {
            PanelState = PanelState.Active;
        }
        
        // 恢复UI动效，禁用输入
        public virtual void OnPause()
        {
            PanelState = PanelState.Paused;
        }
        
        // 开启输入
        public virtual void OnEnableInput(){}
        
        // 禁用输入
        public virtual void OnDisableInput(){}
    }
}