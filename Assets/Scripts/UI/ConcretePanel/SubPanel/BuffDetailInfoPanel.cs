using GameLogic.BUFF;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ConcretePanel.SubPanel
{
    public class BuffDetailInfoPanel : MonoBehaviour
    {
        [SerializeField] private Button closeButton;
        
        [SerializeField] private Text buffNameText;
        [SerializeField] private Image iconImage;
        [SerializeField] private Text durationText;
        [SerializeField] private Text stackCountText;
        [SerializeField] private Text descriptionText;
        
        private BuffInstance _buffInstance;
        
        private void Awake()
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        }

        private void OnCloseButtonClicked()
        {
            gameObject.SetActive(false);
        }

        public void Initialize(BuffInstance buffInstance)
        {
            buffNameText.text = buffInstance.BuffConfig.buffName;
            durationText.text = buffInstance.GetDurationText();
            stackCountText.text = buffInstance.GetStackCountText();
            descriptionText.text = buffInstance.GetDescription();
            iconImage.sprite = buffInstance.BuffConfig.icon;
        }
    }
}