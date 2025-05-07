using System;
using System.Collections;
using System.Collections.Generic;
using GameLogic.Unit.Controller;
using UnityEngine;

namespace GameLogic.TriggerSystem
{
    public class TriggerCondition
    {
        /// <summary>
        /// 能否在一回合中被同一对象多次触发
        /// </summary>
        private readonly bool _canMultiTrigger;
        private readonly HashSet<EntityController> _unitHasTriggered;

        public void ResetUnitHasTriggered() => _unitHasTriggered?.Clear();
        
        public TriggerCondition(bool canMultiTrigger = false)
        {
            _canMultiTrigger = canMultiTrigger;
            if (!_canMultiTrigger) _unitHasTriggered = new HashSet<EntityController>();
        }
        
        public List<EntityController> HasTriggeredList;
        /// <summary>
        /// 触发条件
        /// </summary>
        private Func<EntityController, bool> _condition;
        
        public bool IsSatisfied(EntityController baseEntity)
        {
            if (!_canMultiTrigger && _unitHasTriggered.Contains(baseEntity)) return false;
            // 如果没有条件，直接返回true
            var isSatisfied = _condition?.Invoke(baseEntity) ?? true;
            if (isSatisfied && !_canMultiTrigger) _unitHasTriggered.Add(baseEntity);
            return isSatisfied;
        }

        public void AddCondition(Func<EntityController, bool> condition)
        {
            _condition += condition;
        }
        
        public void RemoveCondition(Func<EntityController, bool> condition)
        {
            _condition -= condition;
        }
    }
    
    public class Trigger
    {
        public int ID;
        public int Duration;
        public int MaxTriggerCount;
        public TriggerCondition Condition;
        
        public int RemainingTurns;
        public int RemainingTriggerCount;
        public bool IsActive = true;

        public Trigger(int duration, int maxTriggerCount)
        {
            // TODO 补充调整初始化所需参数
        }

        public bool NextTurn() {
            RemainingTurns--;
            return RemainingTurns <= 0;
            // HasTriggeredList.Clear();
        }
        
        /// <summary>
        /// 监听者
        /// </summary>
        public EntityController Listener;
        
        /// <summary>
        /// 监听区域
        /// </summary>
        public HashSet<Vector2Int> TriggerPositions;
        
        /// <summary>
        /// 触发具体逻辑
        /// </summary>
        public Func<EntityController, IEnumerator> OnTriggered;
    }
}