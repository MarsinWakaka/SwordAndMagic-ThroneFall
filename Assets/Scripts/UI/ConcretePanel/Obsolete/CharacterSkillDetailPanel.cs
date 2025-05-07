using UnityEngine;
using UnityEngine.UI;

namespace UI.ConcretePanel
{
    public class CharacterSkillDetailParams
    {
        public string SkillId;
    }
    
    public class CharacterSkillDetailPanel : BaseUIPanel
    {
        [SerializeField] private Button backButton;
        
        [SerializeField] private Text skillNameText;
        
        private void Awake()
        {
            backButton.onClick.AddListener(() =>
            {
                UIManager.Instance.ClosePanel(PanelName.CharacterSkillDetailPanel);
            });
        }

        public override void OnCreate(object data)
        {
            base.OnCreate(data);
            if (data is CharacterSkillDetailParams skillDetailParams)
            {
                // var skillDetails = SkillManager.Instance.GetSkillDetails(skillDetailParams.SkillId);
                skillNameText.text = skillDetailParams.SkillId.ToString(); // Replace with actual skill name
            }
            else
            {
                Debug.LogError("Invalid data passed to CharacterSkillDetailPanel");
            }
        }
    }
}