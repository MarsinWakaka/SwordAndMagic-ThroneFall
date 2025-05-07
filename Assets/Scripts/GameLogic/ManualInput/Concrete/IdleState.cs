using GameLogic.Unit.Controller;
using MyFramework.Utilities.Stack;
using UnityEngine;

namespace GameLogic.ManualInput.Concrete
{
    public class IdleState : IPersistentStackNode
    {
        private readonly ManualInputController _manualInputController;
        
        public IdleState(ManualInputController manualInputController)
        {
            _manualInputController = manualInputController;
        }
        
        public void OnEnter(IStackNodeParams parameters = null)
        {
            Debug.Log("[Input Controller] : IdleState Entered");
        }

        public void OnResume()
        {
            _manualInputController.EnableMouseHoverStyle = true;
            _manualInputController.OnHandleRaycastInfo += HandleRaycastInfo;
        }

        public void OnPause()
        {
            _manualInputController.EnableMouseHoverStyle = false;
            _manualInputController.OnHandleRaycastInfo -= HandleRaycastInfo;
        }

        public void OnExit()
        {
            Debug.Log("[Input Controller] : IdleState Exited");
        }

        public void OnUpdate()
        {
            
        }
        
        private void HandleRaycastInfo(RaycastHit2D hitInfo)
        {
            if (hitInfo.collider == null) return;
            
            var unit = hitInfo.collider.GetComponent<CharacterUnitController>();
            if (unit != null)
            {
                if (unit.RuntimeData.faction == _manualInputController.canControlFaction)
                {
                    _manualInputController.StackManager.Push(InputState.MovementSelect, new CharacterActionParam(unit));
                }
                else
                {
                    // TODO 进入侦察状态
                    Debug.Log($"TODO {_manualInputController.canControlFaction} 侦察 {unit.FriendlyInstanceID()} 单位");
                }
                return;
            }
        }
    }
}