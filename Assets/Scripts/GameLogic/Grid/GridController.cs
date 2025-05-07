using System;
using GameLogic.Unit.Controller;
using MyFramework.Utilities;
using UnityEngine;

namespace GameLogic.Grid
{
    public enum GridState
    {
        Normal,
        Hover,
        Selected,
    }

    // 增强版控制器
    public class GridController : MonoBehaviour
    {
        protected static readonly Vector3 HeightOffset = new Vector3(0, 0.25f, 0);
        protected static Color DefaultColor = new Color(1, 1, 1, 1);
        protected static Color HoverColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        protected static Color SelectedColor = new Color(0.6f, 0.6f, 1f, 1f);
        
        private SpriteRenderer _renderer;
        public RuntimeGridData RuntimeData { get; private set; }
        public readonly Bindable<GridState> State = new();
        
        protected virtual void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

#if GRID_DEBUG
        private void OnDrawGizmos()
        {
            // 如果上面有实体，则地块前绘制一个红色圆圈
            if (RuntimeData.EntitiesOnThis != null)
            {
                var pos = transform.position;
                var circlePos = new Vector3(pos.x, pos.y, pos.z - 0.5f);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(circlePos, 0.1f);
            }
            else
            {
                // 如果没有实体，则地块前绘制一个绿色圆圈
                var pos = transform.position;
                var circlePos = new Vector3(pos.x, pos.y, pos.z - 0.5f);
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(circlePos, 0.1f);
            }
        }
#endif


        public void Initialize(RuntimeGridData runtimeGridData)
        {
            RuntimeData = runtimeGridData;
            SetupVisual();
            RegisterEvents();
            HandleGridCoordChange(runtimeGridData.GridCoord.Value);
        }

        private void SetupVisual()
        {
            _renderer.sprite = RuntimeData.ConfigData.BaseSprite;
        }

        private void RegisterEvents()
        {
            State.AddListener(HandleStateChange);
            RuntimeData.GridCoord.AddListener(HandleGridCoordChange);
        }

        private void HandleStateChange(GridState newState)
        {
            _renderer.color = newState switch
            {
                GridState.Hover => HoverColor,
                GridState.Selected => SelectedColor,
                _ => DefaultColor
            };
        }

        private void HandleGridCoordChange(Vector3Int newCoord)
        {
            transform.position = CoordinateConverter.CoordToWorldPos(newCoord) + HeightOffset;
            _renderer.size = new Vector2(_renderer.size.x, 1 + newCoord.z * 0.25f);
        }
        
        public void OnUnitEnter(EntityController entityController)
        {
            if (RuntimeData.EntitiesOnThis == null)
            {
                RuntimeData.EntitiesOnThis = entityController;
                entityController.UpdatePosition(RuntimeData.GridCoord.Value);
            }
            else
            {
                // TODO 处理单位进入时的逻辑
                Debug.LogWarning("Grid already occupied by another unit.");
            }
        }
        
        public void OnUnitLeave(EntityController entityController)
        {
            RuntimeData.EntitiesOnThis = null;
        }
    
        public void OnMouseHover()
        {
            if (State.Value == GridState.Selected) return;
            State.Value = GridState.Hover;
        }
    
        public void OnCancelMouseHover()
        {
            if (State.Value == GridState.Selected) return;
            State.Value = GridState.Normal;
        }
    
        public void OnMouseClicked()
        {
            State.Value = GridState.Selected;
        }
    
        public void OnCancelMouseClicked()
        {
            State.Value = GridState.Normal;
        }

        #region 拓展


        public Vector2Int GetGrid2DCoord()
        {
            var gridCoordValue = RuntimeData.GridCoord.Value;
            return new Vector2Int(gridCoordValue.x, gridCoordValue.y);
        }
        
        public Vector3Int GetGridCoord() => RuntimeData.GridCoord.Value;

        #endregion
    }
}