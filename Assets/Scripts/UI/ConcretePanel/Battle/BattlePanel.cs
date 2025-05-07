using Core.SceneSystem;
using Scene;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ConcretePanel.Battle
{
    public class BattlePanel : BaseUIPanel
    {
        // 左上角用于战斗信息以及设置的按钮
        [SerializeField] private Button battleInfoButton;
        
        // [View] 战斗信息
        [SerializeField] private Text curRoundText;
        [SerializeField] private Text gridInfoText;

        private void Awake()
        {
            battleInfoButton.onClick.AddListener(() =>
            {
                // 直接返回主场景
                SceneLoader.LoadScene(new MainMenuScene());
                // TODO : 制作战斗信息面板
                // UIManager.Instance.ShowPanel(PanelName.BattleInfoPanel, OpenStrategy.HideCurrent);
            });
        }
    }
}