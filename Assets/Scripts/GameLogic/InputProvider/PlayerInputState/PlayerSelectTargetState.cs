// using GameLogic.Grid;
// using GameLogic.Unit.Controller;
// using MyFramework.Utilities.FSM;
// using UnityEngine;
//
// namespace GameLogic.InputProvider.PlayerInputState
// {
//     public class PlayerSelectTargetState : IState
//     {
//         private readonly PlayerInputProvider _inputProvider;
//         private readonly int _rayCastLayer;
//         
//         public PlayerSelectTargetState(PlayerInputProvider inputProvider)
//         {
//             _inputProvider = inputProvider;
//             _rayCastLayer = inputProvider.groundLayer | inputProvider.entityLayer;
//         }
//         
//         public void OnEnter(IStateParams stateParams)
//         {
//             Debug.Log("SelectState OnEnter");
//             _inputProvider.OnHandleRaycastInfo += HandleRaycastInfo;
//         }
//         
//         public void OnUpdate()
//         {
//             _inputProvider.ShowMouseHoverStyleOnGrid();
//             _inputProvider.HandleMouseScroll();
//             
//             if (Input.GetMouseButtonDown(0))
//             {
//                 _inputProvider.RayCastAll(_rayCastLayer);
//             }
//         }
//         
//         public void OnExit(IStateParams stateParams)
//         {
//             _inputProvider.OnHandleRaycastInfo -= HandleRaycastInfo;
//             _inputProvider.SelectedGrid?.OnCancelMouseClicked();
//             _inputProvider.SelectedGrid = null;
//         }
//         
//         private void HandleRaycastInfo(RaycastHit2D hitInfo)
//         {
//             if (hitInfo.collider != null)
//             {
//                 var unit = hitInfo.collider.GetComponent<CharacterUnitController>();
//                 if (unit != null)
//                 {
//                     _inputProvider.Fsm.ChangeState(InputState.MoveAction,
//                         new MovementStateEnterParams(unit)
//                     );
//                     return;
//                 }
//                 
//                 var gridCtrl = hitInfo.collider.GetComponent<GridController>();
//                 if (gridCtrl != null)
//                 {
//                     _inputProvider.SelectedGrid?.OnCancelMouseClicked();
//                     _inputProvider.SelectedGrid = gridCtrl;
//                     _inputProvider.SelectedGrid.OnMouseClicked();
//                 }
//             }
//         }
//     }
// }