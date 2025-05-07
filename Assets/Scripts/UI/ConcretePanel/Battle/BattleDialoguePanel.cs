using System;
using System.Collections;
using Events.Battle;
using GameLogic.Dialogue;
using MyFramework.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ConcretePanel.Battle
{
    public class BattleDialogueParams
    {
        public Dialogue Dialogue;
        public Action DialogueComplete;

        public BattleDialogueParams(Dialogue dialogue, Action dialogueComplete)
        {
            Dialogue = dialogue;
            DialogueComplete = dialogueComplete;
        }
    }
    
    public class BattleDialoguePanel : BaseUIPanel
    {
        private const float CharDelay = 0.06f;
        
        [SerializeField] private Button nextDialogueButton;
        [SerializeField] private Button historyButton;
        [SerializeField] private Button autoButton;
        [SerializeField] private Button skipButton;
        
        [SerializeField] private Text speakerText;
        [SerializeField] private Text dialogueText;
        
        [SerializeField] private Image bgImage;
        [SerializeField] private BattleDialogueImageCtrl[] characterImages;
        private bool _shouldSkip;
        
        private void SetShouldSkip(bool value) => _shouldSkip = value;
        private bool GetShouldSkip() => _shouldSkip;
        
        private void Awake()
        {
            nextDialogueButton.onClick.AddListener(() =>
            {
                SetShouldSkip(true);
                // Debug.Log($"下一句 :{Time.time}");
            });
            historyButton.onClick.AddListener(() => { Debug.Log("历史记录"); });
            autoButton.onClick.AddListener(() => { Debug.Log("自动播放"); });
            skipButton.onClick.AddListener(() => { SetShouldSkip(true); });
        }

        public override void OnCreate(object data)
        {
            base.OnCreate(data);
            // TODO 解析数据
            if (data is BattleDialogueParams param)
            {
                StartCoroutine(ShowDialogue(param.Dialogue, param.DialogueComplete));
            }
            else
            {
                Debug.LogError($"数据类型错误，实际类型为{data.GetType()}");
            }
        }

        private IEnumerator ShowDialogue(Dialogue dialogue, Action onComplete)
        {
            foreach (var node in dialogue.nodes)
            {
                // 处理操作
                foreach (var operation in node.operation)
                {
                    switch (operation.operationType)
                    {
                        case OperationType.CharacterFadeIn:
                            EventBus.Channel(Channel.Gameplay).Publish(
                                new CharacterExpressionRequest(node.speaker, node.expression, (image) =>
                                {
                                    characterImages[operation.intValue].FadeIn(image);
                                }));
                            yield return new WaitForSeconds(operation.floatValue);
                            break;
                        case OperationType.CharacterFadeOut:
                            characterImages[operation.intValue].FadeOut();
                            break;
                        default:
                            EventBus.Channel(Channel.Gameplay).Publish(
                                new DialogueBgImageRequest(operation.strValue, null, (image) =>
                                {
                                    bgImage.sprite = image;
                                }));
                            // Debug.LogError($"未处理的操作类型: {operation.operationType}");
                            break;
                    }
                }
                // 设置说话者
                speakerText.text = node.speaker;
                yield return ShowDialogueText(node.text);
                yield return new WaitUntil(GetShouldSkip); // 等待跳过按钮被点击
                SetShouldSkip(false); // 重置跳过状态
            }
            onComplete?.Invoke();
        }
        
        private IEnumerator ShowDialogueText(string text, float charDelay = CharDelay)
        {
            dialogueText.text = ""; // 初始清空
            foreach (var c in text)
            {
                dialogueText.text += c; // 直接追加字符
                // 检测跳过输入
                if (_shouldSkip)
                {
                    SetShouldSkip(false); // 重置跳过状态
                    dialogueText.text = text; // 直接显示全文
                    yield break; // 终止协程
                }
                yield return new WaitForSeconds(charDelay);
            }
        }
    }
}