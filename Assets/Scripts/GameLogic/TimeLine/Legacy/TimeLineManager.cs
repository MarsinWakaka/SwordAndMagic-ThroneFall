// using System.Collections;
// using System.Collections.Generic;
// using Events;
// using Events.Battle;
// using MyFramework.Utilities;
// using UnityEngine;
//
// namespace GameLogic.TimeLine
// {
//     public class TimeLineManager : MonoBehaviour
//     {
//         private List<TimeLineEvent> _timeLineEvents;
//         
//         private void OnEnable()
//         {
//             EventBus.Channel(Channel.Battle).Subscribe<TriggerSceneEventArgs>(GameEvent.TriggerSceneEvent, HandleStartTurn);
//         }
//         
//         private void OnDisable()
//         {
//         }
//         
//         private void HandleStartTurn(TriggerSceneEventArgs args)
//         {
//             StartCoroutine(OnStartTurn(args));
//         }
//
//         private IEnumerator OnStartTurn(TriggerSceneEventArgs args)
//         {
//             var newTurn = args.NewTurn;
//             foreach (var lineEvent in _timeLineEvents)
//             {
//                 if (lineEvent.triggerCondition.IsSatisfied(newTurn))
//                 {
//                     yield return lineEvent.Execute();
//                 }
//             }
//             args.OnComplete();
//             // SceneEventHandleDone
//             // // 事件排序
//             // _timeLineEvents.Sort((a, b) => a.triggerCondition.roundRange.y.CompareTo(b.triggerCondition.roundRange.y));
//         }
//     }
// }