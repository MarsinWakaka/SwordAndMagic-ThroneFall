// using System.Collections;
// using UnityEngine;
// using UnityEngine.SceneManagement;

namespace Scene
{
    public static class SceneLoader
    {
        // private static Dictionary<string, BaseScene> _scenes = new();
        private static BaseScene _curScene;

        public static void LoadScene(BaseScene newScene)
        {
            _curScene?.OnExit();
            _curScene = newScene;
            _curScene.OnEnter();
        }
    }

    public abstract class BaseScene
    {
        // public abstract string SceneName { get; }

        public abstract void OnEnter();

        public abstract void OnExit();
    }
}

// namespace Scene
// {
//     public class SceneLoader : MonoBehaviour {
//         // 场景配置数据
//         [System.Serializable]
//         public class SceneConfig {
//             public string sceneName;
//             public LoadSceneMode mode = LoadSceneMode.Single;
//             public bool allowActivation = true;
//             public float minLoadingTime = 1f;
//         }
//
//         // 异步加载场景
//         public IEnumerator LoadSceneAsync(SceneConfig config) {
//             AsyncOperation op = SceneManager.LoadSceneAsync(config.sceneName, config.mode);
//             op.allowSceneActivation = config.allowActivation;
//
//             float timer = 0;
//             while (!op.isDone) {
//                 timer += Time.deltaTime;
//                 if (timer >= config.minLoadingTime && op.progress >= 0.9f) {
//                     op.allowSceneActivation = true;
//                 }
//                 yield return null;
//             }
//         }
//     }
//     
//     public interface ISceneHandler {
//         void OnScenePreLoad();
//         void OnSceneLoaded();
//         void OnSceneUnloading();
//     }
//
//     public class SceneLifecycleController {
//         private List<ISceneHandler> _handlers = new();
//
//         // 注册场景处理器
//         public void RegisterHandler(ISceneHandler handler) {
//             if (!_handlers.Contains(handler)) {
//                 _handlers.Add(handler);
//             }
//         }
//
//         // 触发场景加载前事件
//         public void TriggerPreLoad() {
//             foreach (var handler in _handlers) {
//                 handler.OnScenePreLoad();
//             }
//         }
//     }
//     
//     public class SceneStateMachine : MonoBehaviour {
//         private enum SceneState { Idle, Preloading, Loading, PostProcessing }
//         private SceneState _currentState = SceneState.Idle;
//
//         public void RequestSceneChange(SceneConfig config) {
//             if (_currentState != SceneState.Idle) return;
//         
//             StartCoroutine(SceneChangeRoutine(config));
//         }
//
//         private IEnumerator SceneChangeRoutine(SceneConfig config) {
//             _currentState = SceneState.Preloading;
//             yield return HandlePreload(config);
//         
//             _currentState = SceneState.Loading;
//             yield return SceneLoader.Instance.LoadSceneAsync(config);
//         
//             _currentState = SceneState.PostProcessing;
//             yield return HandlePostProcessing();
//         
//             _currentState = SceneState.Idle;
//         }
//     }
//     
//     [CreateAssetMenu(menuName = "Scene/Scene Profile")]
//     public class SceneProfile : ScriptableObject {
//         public string displayName;
//         public SceneReference sceneReference;
//         public SceneType sceneType;
//         public AudioClip bgm;
//         public GameObject loadingScreenPrefab;
//         public List<SceneDependency> dependencies;
//     }
//
//     [System.Serializable]
//     public class SceneDependency {
//         public string bundleName;
//         public int priority;
//     }
// }