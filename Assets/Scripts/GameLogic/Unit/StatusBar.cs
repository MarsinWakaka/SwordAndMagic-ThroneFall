using Core;
using GameLogic.Unit.BattleRuntimeData;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic.Unit
{
    [RequireComponent(typeof(Canvas))]
    public class StatusBar : MonoBehaviour
    {
        [SerializeField] private Image maxHealthUI;
        [SerializeField] private Image curHealthUI;
        [SerializeField] private Image curSkillPointUI;
        private int _maxHealth;
        private int _curHealth;
        private int _curSkillPoint;
        
        private CharacterBattleRuntimeData _runtimeData;

        public void Initialize(CharacterBattleRuntimeData runtimeData)
        {
            GetComponent<Canvas>().worldCamera = Camera.main;
            
            var gameManager = GameManager.Instance;
            var color = runtimeData.faction == Faction.Player ? gameManager.playerColor : gameManager.enemyColor;
            maxHealthUI.color = color;
            curHealthUI.color = color;
            
            _runtimeData = runtimeData;
            _maxHealth = runtimeData.MaxHp.Value;
            _curHealth = runtimeData.CurHp.Value;
            _curSkillPoint = runtimeData.CurSkillPoint.Value;
            OnMaxHealthChanged(_maxHealth);
            OnCurrentHealthChanged(_curHealth);
            OnCurrentSkillPointChanged(_curSkillPoint);

            runtimeData.MaxHp.AddListener(OnMaxHealthChanged);
            runtimeData.CurHp.AddListener(OnCurrentHealthChanged);
            runtimeData.CurSkillPoint.AddListener(OnCurrentSkillPointChanged);
        }
        
        private void OnDestroy()
        {
            if (_runtimeData == null) return;
            _runtimeData.MaxHp.RemoveListener(OnMaxHealthChanged);
            _runtimeData.CurHp.RemoveListener(OnCurrentHealthChanged);
            _runtimeData.CurSkillPoint.RemoveListener(OnCurrentSkillPointChanged);
        }

        #region HP

        private void OnMaxHealthChanged(int maxHealth)
        {
            _maxHealth = maxHealth;
            curHealthUI.fillAmount = (float)_curHealth / _maxHealth;
        }
        
        private void OnCurrentHealthChanged(int currentHealth)
        {
            _curHealth = currentHealth;
            curHealthUI.fillAmount = (float)_curHealth / _maxHealth;
        }

        #endregion

        #region SP

        private void OnCurrentSkillPointChanged(int currentSkillPoint)
        {
            _curSkillPoint = currentSkillPoint;
            curSkillPointUI.fillAmount = (float)_curSkillPoint / _maxHealth;
        }

        #endregion
    }
}