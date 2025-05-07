// using System;
// using UnityEngine;
//
// namespace GameLogic.TimeLine
// {
//     [Serializable]
//     public class TriggerCondition
//     {
//         [Header("限制可以触发的时间范围")]
//         public Vector2Int roundRange = new(1, 999);
//         
//         [Header("回合条件满足时触发")]
//         public int triggerRound;
//         
//         public bool IsSatisfied(int currentRound)
//         {
//             if (currentRound < roundRange.x || currentRound >= roundRange.y) return false;
//             if (triggerRound != 0 && currentRound == triggerRound) return true;
//             
//             return false;
//         }
//     }
// }