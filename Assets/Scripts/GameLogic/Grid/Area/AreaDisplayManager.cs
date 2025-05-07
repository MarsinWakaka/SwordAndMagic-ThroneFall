using System.Collections.Generic;
using Events.Battle;
using MyFramework.Utilities;
using UnityEngine;

namespace GameLogic.Grid.Area
{
    public class AreaDisplayManager : MonoBehaviour
    {
        private readonly Dictionary<AreaType, IAreaDisplay> _areaDisplays = new();
        
        private void Awake()
        {
            var areaDisplays = GetComponentsInChildren<IAreaDisplay>();
            foreach (var areaDisplay in areaDisplays)
            {
                _areaDisplays.Add(areaDisplay.CanHandleAreaType, areaDisplay);
            }
        }

        private void OnEnable()
        {
            EventBus.Channel(Channel.Gameplay).Subscribe<AreaDisplayEvent>(HandleAreaDisplay);
            EventBus.Channel(Channel.Gameplay).Subscribe<AreaHideEvent>(HandleAreaHide);
        }
        
        private void OnDisable()
        {
            EventBus.Channel(Channel.Gameplay).Unsubscribe<AreaDisplayEvent>(HandleAreaDisplay);
            EventBus.Channel(Channel.Gameplay).Unsubscribe<AreaHideEvent>(HandleAreaHide);
        }

        private void HandleAreaDisplay(AreaDisplayEvent displayEvent)
        {
            if (_areaDisplays.TryGetValue(displayEvent.AreaType, out var areaDisplay))
            {
                if (displayEvent.HideOldFirst) areaDisplay.Hide();
                areaDisplay.Display(displayEvent.Coords);
            } 
            else
            {
                Debug.LogError($"No area display found for area type: {displayEvent.AreaType}");
            }
        }
        
        private void HandleAreaHide(AreaHideEvent hideEvent)
        {
            if (_areaDisplays.TryGetValue(hideEvent.AreaType, out var areaDisplay))
            {
                areaDisplay.Hide();
            } 
            else
            {
                Debug.LogError($"No area display found for area type: {hideEvent.AreaType}");
            }
        }
    }
}