using System;
using Config;
using GameLogic.Battle;
using SoundSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.ConcretePanel.Battle
{
    public class DeploySlot : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image unitIcon;
        [SerializeField] private Text unitLevelText;
        
        private DeployCharacterData _characterData;
        public event Action<Vector3, DeployCharacterData> OnClickSlot;

        public void InitializeSlot(DeployCharacterData data)
        {
            if (data == null) {
                Debug.LogError("DeployCharacterData is null");
                Destroy(gameObject);
            }
            else
            {
                _characterData = data;
                SetSelectable(data.CanUse.Value);
                // 监听
                data.CanUse.AddListener(SetSelectable);
                unitLevelText.text = $"Lv.{data.Data.level}";
                unitIcon.sprite = CharacterConfigManager.Instance.GetConfig(data.Data.characterID).icon;
            }
        }
        
        private void OnDestroy()
        {
            if (_characterData == null) return;
            // 取消监听
            _characterData.CanUse.RemoveListener(SetSelectable);
        }

        private bool _selectable;

        private void SetSelectable(bool selectable)
        {
            _selectable = selectable;
            unitIcon.color = selectable ? Color.white : Color.gray;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log($"Slot clicked: {_characterData.Data.characterID}");
            if (!_selectable) return;
            SoundManager.Instance.PlaySFXOneShot(SFX.ButtonNormalClick);
            OnClickSlot?.Invoke(transform.position, _characterData);
        }
    }
}