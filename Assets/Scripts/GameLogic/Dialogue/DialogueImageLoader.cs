using Events.Battle;
using MyFramework.Utilities;
using UnityEngine;

namespace GameLogic.Dialogue
{
    public class DialogueImageLoader : MonoBehaviour
    {
        private const string ExpressionPath = "Dialogues/Expressions/";
        private const string BgPath = "Dialogues/Background/";
        
        private void OnEnable()
        {
            // TODO : 订阅事件
            EventBus.Channel(Channel.Gameplay).Subscribe<CharacterExpressionRequest>(HandleExpressionRequest);
            EventBus.Channel(Channel.Gameplay).Subscribe<DialogueBgImageRequest>(HandleBgImageRequest);
        }

        private void OnDisable()
        {
            // TODO : 注销事件
            EventBus.Channel(Channel.Gameplay).Unsubscribe<CharacterExpressionRequest>(HandleExpressionRequest);
            EventBus.Channel(Channel.Gameplay).Unsubscribe<DialogueBgImageRequest>(HandleBgImageRequest);
        }
        
        private void HandleExpressionRequest(CharacterExpressionRequest request)
        {
            var image = Load<Sprite>($"{ExpressionPath}{request.CharacterName}_{request.Expression}");
            if (image != null) request.OnImageReady?.Invoke(image);
        }

        private void HandleBgImageRequest(DialogueBgImageRequest request)
        {
            var bg = Load<Sprite>(BgPath + request.ImageID);
            if (bg != null) request.OnImageLoaded(bg);
        }

        private T Load<T>(string fullPath) where T : Object
        {
            T result = Resources.Load<T>(fullPath);
            if (result == null)
            {
                Debug.LogError($"资源加载失败{fullPath} : {typeof(T)}");
            }
            return result;
        }
    }
}