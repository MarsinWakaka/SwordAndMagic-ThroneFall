using System;
using GameLogic.BUFF;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.UISlots
{
    public class BuffSlot : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image iconImage;
        
        private BuffInstance _buffInstance;
        public event Action<BuffInstance> OnBuffSlotClicked;

        public void Initialize(BuffInstance buffInstance)
        {
            _buffInstance = buffInstance;
            iconImage.sprite = buffInstance.BuffConfig.icon;
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            OnBuffSlotClicked?.Invoke(_buffInstance);
        }
    }
}