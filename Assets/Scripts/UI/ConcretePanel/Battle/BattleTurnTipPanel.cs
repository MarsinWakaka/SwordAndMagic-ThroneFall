using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ConcretePanel.Battle
{
    public class TurnTipPanelParam
    {
        public readonly string TipText;
        public float Duration;
        
        public Action OnComplete;

        public TurnTipPanelParam(string tipText, Action onComplete, float duration = 1f)
        {
            TipText = tipText;
            Duration = duration;
            OnComplete = onComplete;
        }
    }
    
    public class BattleTurnTipPanel : BaseUIPanel
    {
        [SerializeField] CanvasGroup canvasGroup;
        RectTransform rectTransform;
        [SerializeField] private Text tipText;
        [SerializeField] private Vector2 startPos;
        [SerializeField] private Vector2 endPos;
        
        private Action _onComplete;

        public override void OnCreate(object data)
        {
            base.OnCreate(data);
            if (data is TurnTipPanelParam param)
            {
                tipText.text = param.TipText;
                _onComplete = param.OnComplete;
                // 使用DoTween来实现移动和淡出效果
                rectTransform = canvasGroup.GetComponent<RectTransform>();
                rectTransform.localPosition = startPos;
                var sequence = DOTween.Sequence();
                sequence.Append(rectTransform.DOLocalMove(endPos, param.Duration));
                sequence.Join(canvasGroup.DOFade(1f, param.Duration));
                sequence.AppendInterval(param.Duration);
                sequence.AppendCallback(() =>
                {
                    canvasGroup.DOFade(0f, param.Duration).OnComplete(() =>
                    {
                        _onComplete?.Invoke();
                    });
                });
            }
            else
            {
                Debug.LogError("Invalid data passed to TurnTipPanel");
            }
        }
        
        public override void OnRelease()
        {
            base.OnRelease();
            tipText.text = string.Empty;
        }
    }
}