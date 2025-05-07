// 增强版运行时数据类

using System.Collections.Generic;
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
        public StaticGridData ConfigData => GridConfigManager.Instance.GetConfig(_cellID);
    }
}