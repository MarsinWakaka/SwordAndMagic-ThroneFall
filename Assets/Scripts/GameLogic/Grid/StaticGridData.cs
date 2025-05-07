using UnityEngine;
using UnityEngine.Serialization;

namespace GameLogic.Grid
{
    [CreateAssetMenu(fileName = "NewCellData", menuName = "CellData", order = 0)]
    public class StaticGridData : ScriptableObject
    {
        [SerializeField] private string cellID;
        [SerializeField] private string cellName;
        [SerializeField] private Sprite baseSprite;
        [SerializeField] private bool blockMovement;
        
        public string CellID => cellID;
        public string CellName => cellName;
        public Sprite BaseSprite => baseSprite;
        public bool BlockMovement => blockMovement;
        
        public override string ToString()
        {
            return $"Cell ID: {cellID}, Name: {cellName}";
        }
    }
}