using Core.SceneSystem;
using Scene;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic.LevelSystem
{
    public class LevelSlot : MonoBehaviour
    {
        public string levelID;
        
        [SerializeField] private Text levelTitleText;
        [SerializeField] private Image levelProgressBar;
        
        [SerializeField] private Image levelImage;
        [SerializeField] private Button levelButton;
        
        public void Initialize(string levelName, int stars, bool unlocked = false)
        {
            levelTitleText.text = levelName;
            levelProgressBar.fillAmount = (float)stars / 3;
            
            levelButton.onClick.AddListener(OnLevelButtonClick);
            levelButton.interactable = true;
            // levelButton.interactable = unlocked;
        }
        
        private void OnLevelButtonClick()
        {
            Debug.Log($"加载关卡 {levelID}");
            // TODO 等待玩家确认后加载
            SceneLoader.LoadScene(new BattleScene(levelID));
        }
    }
}