using System;
using System.Collections;
using GameLogic.Unit.Controller;
using UnityEngine;

public enum BuffStackMethod
{
    Separate,
    Merge
}

// BUFF的消耗规则
public enum BuffConsumeRule
{
    SelfTurnStart,
    SmallTurnEnd,
    BigTurnEnd,
    AfterTrigger,
    NotConsume
}

namespace GameLogic.BUFF
{
    [Serializable]
    public abstract class Buff : ScriptableObject
    {
        public string buffID;
        public string buffType;
        public string buffName;
        public Sprite icon;
        
        // 基于优先级依次触发
        public int priority;
        
        public abstract string Description{ get; set; }
        
        public bool isPermanent;
        // 是否分开计算剩余时间
        public BuffStackMethod stackMethod;
        
        [Range(1, 10)]
        public int maxStackCount;

        public abstract BuffInstance CreateBuffInstance(CharacterUnitController owner);
    }
}