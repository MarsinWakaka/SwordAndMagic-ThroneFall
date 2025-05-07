using Core.SaveSystem;
using MyFramework.Utilities;
using MyFramework.Utilities.Singleton;
using SaveSystem;
using UI;
using UnityEngine;

namespace Core
{
    public class GameManager : ThreadSafeMonoSingleton<GameManager>
    {
        // TODO : 这里的颜色应该是从配置文件中读取的
        public Color playerColor;
        public Color enemyColor;

        private void Start()
        {
            UIManager.Instance.ShowPanel(PanelName.MainMenuPanel, OpenStrategy.CloseAllFirst);
        }

        protected override void WhenEnable()
        {
            ServiceLocator.Register<ISaveDataProvider>(new TemporarySaveDataProvider());
        }

        protected override void WhenDisable()
        {
            ServiceLocator.UnRegister<ISaveDataProvider>();
        }
    }
}