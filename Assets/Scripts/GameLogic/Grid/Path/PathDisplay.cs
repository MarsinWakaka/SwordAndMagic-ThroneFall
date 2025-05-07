using System.Collections.Generic;
using Events;
using Events.Battle;
using MyFramework.Utilities;
using UnityEngine;
using UnityEngine.Pool;
using Utilities;

namespace GameLogic.Grid.Path
{
    public class PathDisplay : MonoBehaviour
    {
        private IGridManager _gridData;

        private void Start()
        {
            InitializePathWay();
            _gridData = ServiceLocator.Resolve<IGridManager>();
            if (_gridData == null)
            {
                Debug.LogError("GridDataProvider not found.");
            }
        }

        private void OnEnable()
        {
            EventBus.Channel(Channel.Gameplay).Subscribe<DisplayPathWayEvent>(ShowPathWay);
            EventBus.Channel(Channel.Gameplay).Subscribe<HidePathwayEvent>(HandleHidePathWay);
        }
        
        private void OnDisable()
        {
            EventBus.Channel(Channel.Gameplay).Unsubscribe<DisplayPathWayEvent>(ShowPathWay);
            EventBus.Channel(Channel.Gameplay).Unsubscribe<HidePathwayEvent>(HandleHidePathWay);
        }
        

        #region PathWay

        [SerializeField] private Sprite upDirArrow;
        [SerializeField] private Sprite rightDirArrow;
        [SerializeField] private Sprite downDirArrow;
        [SerializeField] private Sprite leftDirArrow;

        private Sprite GetDirectionPathWay(Direction dir)
        {
            return dir switch
            {
                Direction.Up => upDirArrow,
                Direction.Right => rightDirArrow,
                Direction.Down => downDirArrow,
                Direction.Left => leftDirArrow,
                _ => null
            };
        }

        private static Vector3 PathWayOffset => new Vector3(0, 0, -0.02f);
        [SerializeField] private Transform pathWayRoot;
        [SerializeField] private SpriteRenderer arrowPrefab;
        private IObjectPool<SpriteRenderer> _pathWayPool;
        private List<SpriteRenderer> _activePathWay;

        private void InitializePathWay()
        {
            _activePathWay = new List<SpriteRenderer>();
            _pathWayPool = new ObjectPool<SpriteRenderer>(
                () => Instantiate(arrowPrefab, pathWayRoot),
                pathWay => { pathWay.gameObject.SetActive(true); },
                pathWay => { pathWay.gameObject.SetActive(false); });
        }

        private void ShowPathWay(List<Vector2Int> path)
        {
            ClearAllPathWay();
            for(var i = 0; i < path.Count - 1; i++)
            {
                var pathWay = _pathWayPool.Get();
                var direction = DirectionUtil.GetDirection(path[i], path[i + 1]);
                pathWay.sprite = GetDirectionPathWay(direction);
                pathWay.transform.position = _gridData.GetWorldPosition(path[i]) + PathWayOffset;
                _activePathWay.Add(pathWay);
            }
        }
        
        public void ShowPathWay(DisplayPathWayEvent @event)
        {
            ShowPathWay(@event.PathTreeNode.ToPathWayList());
        }
        
        private void HandleHidePathWay(HidePathwayEvent hidePathwayEvent) => ClearAllPathWay();

        private void ClearAllPathWay()
        {
            foreach (var pathWay in _activePathWay)
            {
                _pathWayPool.Release(pathWay);
            }
            _activePathWay.Clear();
        }

        #endregion
    }
}