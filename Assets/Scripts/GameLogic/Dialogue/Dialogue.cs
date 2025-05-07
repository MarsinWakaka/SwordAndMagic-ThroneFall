using System.Collections.Generic;
using UnityEngine;

namespace GameLogic.Dialogue
{
    public enum CharacterExpression
    {
        Normal,
        Smile,
        Laugh,
        Happy,
        Surprise,
        Stun,
        Scared,
        Sad,
        Angry,
        Cry,
    }
    
    public enum OperationType
    {
        // 角色淡出
        [Tooltip("参数说明：int:{0 - 左 1 - 右}")]
        CharacterFadeIn,
        // 角色淡入
        [Tooltip("参数说明：int:{0 - 左 1 - 右}")]
        CharacterFadeOut,
        // 角色表情
        [Tooltip("参数说明：str:{背景图片名称} float:{等待时间}")]
        LoadBackground
    }

    [System.Serializable]
    public class DialogueOperation
    {
        public OperationType operationType;
        
        public string strValue;
        public float floatValue;
        public int intValue;
    }
    
    [System.Serializable]
    public class DialogueNode
    {
        public string speaker;
        
        [TextArea] 
        public string text;
        
        public CharacterExpression expression;
        
        public AudioClip voiceClip;
        
        public DialogueOperation[] operation;
    }
    
    [CreateAssetMenu(menuName = "Battle/Dialogue", order = 1)]
    public class Dialogue : ScriptableObject
    {
        public List<DialogueNode> nodes;
    }
}