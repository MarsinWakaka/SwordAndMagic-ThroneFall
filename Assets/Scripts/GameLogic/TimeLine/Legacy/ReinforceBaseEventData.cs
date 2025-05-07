// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// namespace GameLogic.TimeLine
// {
//     [CreateAssetMenu(menuName = "Battle/TimeLine/ReinforceEvent", order = 1)]
//     public class ReinforceBaseEventData : TimeLineEvent
//     {
//         [Serializable]
//         public struct ReinforceData    
//         {
//             public int unitId;
//             public Vector2Int gridCoord;
//         }
//         
//         public List<ReinforceData> reinforceData;
//         
//         public override IEnumerator Execute()
//         {
//             throw new NotImplementedException();
//         }
//     }
// }