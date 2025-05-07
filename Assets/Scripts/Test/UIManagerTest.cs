using UI;
using UnityEngine;

namespace Test
{
    public class UIManagerTest : MonoBehaviour
    {
        [SerializeField] private bool enable;
        [SerializeField] private string panelName;
        [SerializeField] private OpenStrategy openStrategy;
        private void Start()
        {
            // Test the UIManager functionality
            if (!enable) return;
            UIManager.Instance.ShowPanel(panelName, openStrategy);
            
            // // Simulate button clicks to test panel transitions
            // UIManager.Instance.ShowPanel(PanelName.MainLobbyPanel, OpenStrategy.HideCurrent);
            // UIManager.Instance.ShowPanel(PanelName.CharacterPanel, OpenStrategy.HideCurrent);
            // UIManager.Instance.ClosePanel(PanelName.CharacterPanel);
            // UIManager.Instance.ClosePanel(PanelName.MainLobbyPanel);
        }

        [ContextMenu("Open Panel")]
        private void OpenPanel()
        {
            UIManager.Instance.ShowPanel(panelName, openStrategy);
        }
        
        [ContextMenu("Close Panel")]
        private void ClosePanel()
        {
            UIManager.Instance.ClosePanel(panelName);
        }
    }
}