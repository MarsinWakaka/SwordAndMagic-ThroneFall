using System;
using GameLogic.Unit.Controller;
using UnityEngine;

namespace GameLogic.BUFF
{
    [Serializable]
    public abstract class BuffConfig : ScriptableObject
    {
        /// <summary>
        /// 返回的ID是buff的唯一标识符，ID为文件名
        /// </summary>
        public string BuffID => name;
        // public string buffType;
        public string buffName;
        public string description;
        public Sprite icon;
        
        [Space]
        
        // 基于优先级依次触发
        [Range(-20, 20)]
        public int priority;
        
        // public bool isPermanent;
        // 是否分开计算剩余时间
        // public BuffStackMethod stackMethod;

        [Header("0-表示不启用")]
        [Range(0, 20)] public int maxDuration;
        [Range(0, 10)] public int maxStackCount;

        public abstract BuffInstance CreateBuffInstance(CharacterUnitController owner);
    }
}