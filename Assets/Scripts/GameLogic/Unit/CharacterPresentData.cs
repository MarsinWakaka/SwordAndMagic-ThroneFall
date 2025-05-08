using UnityEngine;

namespace GameLogic.Unit
{
    /// <summary>
    /// [Purpose]
    /// 用户配置角色预设数据
    /// </summary>
    [CreateAssetMenu(fileName = "new PresentData", menuName = "Character/PresentData", order = 1)]
    public class CharacterPresentData : ScriptableObject
    {
        /// <summary>
        /// 配置文件中的角色数据
        /// </summary>
        [SerializeField] private CharacterData characterData;
        
        public CharacterData CharacterDataConfig
        {
            get => characterData;
            set => characterData = value;
        }
    }
}