// 增强版运行时数据类

using GameLogic.Unit.Controller;
using MyFramework.Utilities;
using UnityEngine;

namespace GameLogic.Grid
{
    public class RuntimeGridData
    {
        private readonly string _cellID;
        public readonly Bindable<Vector3Int> GridCoord;
        public EntityController EntitiesOnThis;

        public RuntimeGridData(string cellID, Vector3Int gridCoord)
        {
            _cellID = cellID;
            GridCoord = new Bindable<Vector3Int>(gridCoord);
        }

        private GridDataConfig _dataConfig;

        public GridDataConfig ConfigDataConfig
        {
            get
            {
                if (_dataConfig != null) return _dataConfig;
                _dataConfig = GridConfigManager.Instance.GetConfig(_cellID);
                return _dataConfig;
            }
        }
    }
}