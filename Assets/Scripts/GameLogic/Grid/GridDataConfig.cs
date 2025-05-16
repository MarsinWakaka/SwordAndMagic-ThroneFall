using UnityEngine;

namespace GameLogic.Grid
{
    [CreateAssetMenu(fileName = "NewGridData", menuName = "GamePlay/GridData", order = 0)]
    public class GridDataConfig : ScriptableObject
    {
        [SerializeField] private string cellID;
        [SerializeField] private string cellName;
        [SerializeField] private Sprite baseSprite;
        [SerializeField] private bool walkable;
        
        public string CellID => cellID;
        public string CellName => cellName;
        public Sprite BaseSprite => baseSprite;
        public bool Walkable => walkable;
        
        public override string ToString()
        {
            return $"Cell ID: {cellID}, Name: {cellName}";
        }
    }
}