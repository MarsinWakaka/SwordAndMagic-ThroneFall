using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ConcretePanel.Battle
{
    public class BattleDialogueImageCtrl : MonoBehaviour
    {
        private Image _image;
        
        private void Awake()
        {
            _image = GetComponent<Image>();
            if (_image == null)
            {
                Debug.LogError("Image component not found on the GameObject.");
            }
            _image.color = new Color(1, 1, 1, 0);
        }
        
        public void FadeIn(Sprite sprite, float duration = 0.1f)
        {
            _image.color = new Color(1, 1, 1, 0);
            _image.sprite = sprite;
            _image.DOFade(1, duration);
        }
        
        public void FadeOut(float duration = 0.1f)
        {
            _image.DOFade(0, duration).OnComplete(() => { _image.sprite = null; });
        }
    }
}