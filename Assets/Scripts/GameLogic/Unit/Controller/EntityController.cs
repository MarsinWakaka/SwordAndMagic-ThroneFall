using System.Collections;
using Events.Battle;
using GameLogic.Character.BattleRuntimeData;
using GameLogic.GameAction.Attack;
using GameLogic.Grid;
using GameLogic.TimeLine;
using MyFramework.Utilities;
using UnityEngine;

namespace GameLogic.Unit.Controller
{
    public abstract class EntityController : MonoBehaviour, ITakeDamageableEntity
    {
        protected Transform Trans;
        public Animator Animator { get; private set; }
        public SpriteRenderer Renderer { get; private set; }

        public EntityBattleRuntimeData RuntimeData { get; protected set; }
        
        public void Initialize(EntityBattleRuntimeData entityRuntimeData) 
        {
            RuntimeData = entityRuntimeData;
            // ServiceLocator.Resolve<IUnitManager>().MoveUnit(this, RuntimeData.gridCoord, RuntimeData.gridCoord);
        }
        
        protected virtual void Awake()
        {
            Trans = transform;
            Animator = GetComponent<Animator>();
            Renderer = GetComponent<SpriteRenderer>();
        }

        public void UpdatePosition(Vector3Int newCoord)
        {
            RuntimeData.gridCoord.x = newCoord.x;
            RuntimeData.gridCoord.y = newCoord.y;
            Trans.position = CoordinateConverter.CoordToWorldPos(newCoord);
        }

        protected class TeleportWrapper
        {
            public Vector2Int OldCoord;
            public Vector2Int NewCoord;
            public EntityController Entity;
            
            public TeleportWrapper(EntityController entity)
            {
                Entity = entity;
            }
            
            public IEnumerator TeleportCoroutine()
            {
                var gridManager = ServiceLocator.Resolve<IGridManager>();
                gridManager.MoveUnitFromAToB(Entity, OldCoord, NewCoord);
                yield return new WaitForSeconds(1f);
            }
        }

        protected TeleportWrapper TeleportExecutor;
        
        /// <summary>
        /// 传送单位到指定位置
        /// </summary>
        public void Teleport(Vector2Int newCoord)
        {
            TeleportExecutor ??= new TeleportWrapper(this);
            TeleportExecutor.OldCoord = RuntimeData.gridCoord;
            TeleportExecutor.NewCoord = newCoord;
            TimeLineManager.Instance.AddPerform(TeleportExecutor.TeleportCoroutine);
        }
        
        // private IEnumerator TeleportCoroutine(Vector2Int newCoord)
        // {
        //     var gridManager = ServiceLocator.Resolve<IGridManager>();
        //     gridManager.MoveUnitFromAToB(this, RuntimeData.gridCoord, newCoord);
        //     yield return null;
        // }

        public virtual void Destroy()
        {
            
        }
        
        // public EntitySnapshot CreateSnapshot()
        // {
        //     // return new EntitySnapshot
        //     // {
        //     //     StaticData = StaticData,
        //     //     RuntimeData = RuntimeData,
        //     //     Position = RuntimeData.gridCoord,
        //     //     Direction = RuntimeData.dir
        //     // };
        //     return null;
        // }
        //
        // public void RestoreSnapshot(EntitySnapshot snapshot)
        // {
        //     // Teleport(snapshot.Position);
        //     // RuntimeData.dir = snapshot.Direction;
        //     // StaticData = snapshot.StaticData;
        //     // RuntimeData = snapshot.RuntimeData;
        // }
        
        public virtual string FriendlyInstanceID()
        {
            return $"{RuntimeData.EntityID}-{RuntimeData.InstanceID}";
        }

        public string InstanceID() => RuntimeData.InstanceID;

        public Vector2Int GetCoordinate() => RuntimeData.gridCoord;

        public virtual void TakeDamage(DamageSegment damageSegment) {}

        public GridController GetGridController() =>
            ServiceLocator.Resolve<IGridManager>().GetGridController(GetCoordinate());
    }
}