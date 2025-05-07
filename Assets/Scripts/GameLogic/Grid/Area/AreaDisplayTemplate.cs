using System.Collections.Generic;
using MyFramework.Utilities;
using UnityEngine;
using UnityEngine.Pool;

namespace GameLogic.Grid.Area
{
    public class AreaDisplayTemplate : MonoBehaviour, IAreaDisplay
    {
        [SerializeField] private AreaType areaType;
        [SerializeField] private Transform areaPrefab;
        [SerializeField] private Vector3 offset;
        
        private IObjectPool<Transform> _reachableAreaPool;
        private readonly List<Transform> _areaObjects = new();
         
        private Transform _areaParent;
        
        public AreaType CanHandleAreaType => areaType;
        private void Awake()
        {
            _areaParent = transform;
            _reachableAreaPool = new ObjectPool<Transform>(
                () => Instantiate(areaPrefab, _areaParent),
                area => { area.gameObject.SetActive(true); },
                area => { area.gameObject.SetActive(false); });
        }
        
        public void Display(IEnumerable<Vector2Int> area)
        {
            var dataProvider = ServiceLocator.Resolve<IGridManager>();
            foreach (var gridCoord in area)
            {
                var obj = _reachableAreaPool.Get();
                obj.transform.position = dataProvider.GetWorldPosition(gridCoord) + offset;
                _areaObjects.Add(obj);
            }
        }

        public void Hide()
        {
            if (_areaObjects.Count == 0) return;
            foreach (var area in _areaObjects)
            {
                _reachableAreaPool.Release(area);
            }
            _areaObjects.Clear();
        }
    }
}