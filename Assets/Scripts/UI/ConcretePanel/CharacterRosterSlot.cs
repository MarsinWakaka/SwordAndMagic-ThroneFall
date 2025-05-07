using System;
using Config;
using Events.Global;
using GameLogic.Unit;
using MyFramework.Utilities;
using SoundSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.ConcretePanel
{
    public class CharacterRosterSlot : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image characterImage;
        [SerializeField] private Text characterNameText;
        [SerializeField] private Text characterLevelText;
        
        public event Action<CharacterData> OnClickAction;
        private CharacterData _characterData;

        public void Initialize(CharacterData characterData)
        {
            if (characterData == null)
            {
                Debug.LogError("CharacterData is null");
                Destroy(gameObject);
                return;
            }
            
            _characterData = characterData;
            characterNameText.text = characterData.characterID;
            characterLevelText.text = $"Lv.{characterData.level}";
            characterImage.sprite = CharacterConfigManager.Instance.GetConfig(characterData.characterID).icon;
        }
        
        private void OnEnable()
        {
            RegisterEvent();
        }
        
        private void OnDisable()
        {
            UnregisterEvent();
        }
        
        private void RegisterEvent()
        {
            EventBus.Channel(Channel.Global).Subscribe<CharacterUpgradeSuccessEvent>(HandleCharacterUpgradeSuccess);
        }

        private void HandleCharacterUpgradeSuccess(CharacterUpgradeSuccessEvent evt)
        {
            // 如果接受到角色数据更新来自自己，则更新。
            if (evt.CharacterData.characterID == _characterData.characterID)
            {
                characterLevelText.text = $"Lv.{_characterData.level}";
            }
        }

        private void UnregisterEvent()
        {
            EventBus.Channel(Channel.Global).Unsubscribe<CharacterUpgradeSuccessEvent>(HandleCharacterUpgradeSuccess);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SoundManager.Instance.PlaySFXOneShot(SFX.ButtonNormalClick);
            OnClickAction?.Invoke(_characterData);
        }
    }
}