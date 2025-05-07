using System;
using System.Collections;
using Player;
using SoundSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ConcretePanel.Battle
{
    public class BattleVictoryParams
    {
        public int ResGained;
        public readonly Action VictoryConfirmed;

        public BattleVictoryParams(int resGained, Action victoryConfirmed)
        {
            ResGained = resGained;
            VictoryConfirmed = victoryConfirmed;
        }
    }
    
    public class BattleVictoryPanel : BaseUIPanel
    {
        [SerializeField] private Button confirmButton;
        [SerializeField] private Text resCountText;
        private Action _victoryConfirmed;
        
        private void Awake()
        {
            confirmButton.onClick.AddListener(OnConfirmButtonClicked);
        }

        public override void OnCreate(object data)
        {
            if (data is not BattleVictoryParams param)
            {
                Debug.LogError("Invalid data for BattleVictoryPanel");
                return;
            }
            
            SoundManager.Instance.PlayBGM(BGM.BattleVictoryBGM);
            _victoryConfirmed = param.VictoryConfirmed;
            StartCoroutine(GainResEffect(param.ResGained));
        }

        private IEnumerator GainResEffect(int resGained)
        {
            // var initRes = PlayerDataManager.Instance.Resources;
            resCountText.text = "0";
            const float duration = 2f;
            float curTime = 0;
            var rate = curTime / duration;
            while (rate < 1)
            {
                curTime += Time.deltaTime;
                rate = curTime / duration;
                if (rate > 1f) rate = 1;
                resCountText.text = Mathf.FloorToInt(rate * rate * resGained).ToString();
                yield return null;
            }
        }

        private void OnConfirmButtonClicked()
        {
            SoundManager.Instance.PlaySFXOneShot(SFX.ButtonNormalClick);
            _victoryConfirmed?.Invoke();
        }
    }
}