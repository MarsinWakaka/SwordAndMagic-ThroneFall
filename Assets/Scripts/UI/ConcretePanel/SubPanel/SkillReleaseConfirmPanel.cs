using Events.Battle.Skill;
using GameLogic.Skill.Active;
using MyFramework.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ConcretePanel.SubPanel
{
    public class SkillReleaseConfirmPanelParams
    {
        public ActiveSkillInstance ActiveSkillInstance { get; }
        
        public SkillReleaseConfirmPanelParams(ActiveSkillInstance activeSkillInstance)
        {
            ActiveSkillInstance = activeSkillInstance;
        }
    }
    
    public class SkillReleaseConfirmPanel : BaseUIPanel
    {
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;
        
        [SerializeField] private Text skillNameText;
        [SerializeField] private Text skillSpCostText;
        [SerializeField] private Text skillCdTimeText;
        [SerializeField] private Text skillDescriptionText;

        public override void OnCreate(object data)
        {
            base.OnCreate(data);
            if (data is SkillReleaseConfirmPanelParams panelParams)
            {
                SetActiveSkill(panelParams.ActiveSkillInstance);
            }
            else
            {
                Debug.LogError("Invalid data for SkillReleaseConfirmPanel");
            }
        }

        private void OnEnable()
        {
            confirmButton.onClick.AddListener(OnConfirmButtonClicked);
            cancelButton.onClick.AddListener(OnCancelButtonClicked);
            confirmButton.enabled = false;
            EventBus.Channel(Channel.Gameplay).Subscribe<SkillTargetSelectedUpdateUIEvent>(SkillTargetSelectedUpdateUI);
        }
        
        private void OnDisable()
        {
            EventBus.Channel(Channel.Gameplay).Unsubscribe<SkillTargetSelectedUpdateUIEvent>(SkillTargetSelectedUpdateUI);
            confirmButton.enabled = false;
            confirmButton.onClick.RemoveListener(OnConfirmButtonClicked);
            cancelButton.onClick.RemoveListener(OnCancelButtonClicked);
        }
        
        public void SetActiveSkill(ActiveSkillInstance skillInstance)
        {
            if (skillInstance == null)
            {
                gameObject.SetActive(false);
                return;
            }
            SetVisual(skillInstance);
            gameObject.SetActive(true);
        }
        
        private void SetVisual(ActiveSkillInstance skillInstance)
        {
            var config = skillInstance.ActiveConfig;
            var skillCost = skillInstance.SKillData.skillPointCost;
            var cdTime = skillInstance.maxCooldown;
            skillNameText.text = config.skillName;
            skillSpCostText.text = skillCost == 0 ? "--" : skillCost.ToString();
            skillCdTimeText.text = cdTime == 0 ? "--" : cdTime.ToString();
            skillDescriptionText.text = skillInstance.GetDescription();
        }

        private void SkillTargetSelectedUpdateUI(SkillTargetSelectedUpdateUIEvent updateUIEvents)
        {
            confirmButton.enabled = updateUIEvents.HasTargetSelected;
        }

        private void OnCancelButtonClicked()
        {
            EventBus.Channel(Channel.Gameplay).Publish(new CancelSkillReleaseInput());
        }

        private void OnConfirmButtonClicked()
        {
            EventBus.Channel(Channel.Gameplay).Publish(new SkillReleaseConfirmInput());
        }
    }
}