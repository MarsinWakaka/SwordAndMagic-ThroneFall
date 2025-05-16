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
        
        [Header("UI控件")]
        [SerializeField] private Button historyButton;
        [SerializeField] private Button autoButton;
        [SerializeField] private Button skipButton;
        [SerializeField] private Button nextDialogueButton;
        
        [Header("对话框控件")]
        [SerializeField] private Text speakerText;
        [SerializeField] private Text dialogueText;
        
        [Header("图片显示")]
        [SerializeField] private Image bgImage;
        [SerializeField] private BattleDialogueImageCtrl[] characterImages;
        private bool _showNext;
        
        private void SetShowNext(bool value) => _showNext = value;
        private bool ShowNext() => _showNext;
        
        private BattleDialogueParams _dialogueParams;
        
        
        private void Awake()
        {
            nextDialogueButton.onClick.AddListener(() =>
            {
                SetShowNext(true);
                // Debug.Log($"下一句 :{Time.time}");
            });
            historyButton.onClick.AddListener(() => { Debug.Log("历史记录"); });
            autoButton.onClick.AddListener(() => { Debug.Log("自动播放"); });
            historyButton.enabled = false;
            autoButton.enabled = false;
            skipButton.onClick.AddListener(() =>
            {
                StopAllCoroutines();
                _dialogueParams.DialogueComplete?.Invoke();
            });
        }

        public override void OnCreate(object data)
        {
            base.OnCreate(data);
            // TODO 解析数据
            if (data is BattleDialogueParams param)
            {
                _dialogueParams = param;
                StartCoroutine(ShowDialogue());
            }
            else
            {
                Debug.LogError($"数据类型错误，实际类型为{data.GetType()}");
            }
        }

        private IEnumerator ShowDialogue()
        {
            var dialogue = _dialogueParams.Dialogue;
            var onComplete = _dialogueParams.DialogueComplete;
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
                yield return new WaitUntil(ShowNext); // 等待下一句
                SetShowNext(false);
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
                if (_showNext)
                {
                    SetShowNext(false); // 重置跳过状态
                    dialogueText.text = text; // 直接显示全文
                    yield break; // 终止协程
                }
                yield return new WaitForSeconds(charDelay);
            }
        }
    }
}