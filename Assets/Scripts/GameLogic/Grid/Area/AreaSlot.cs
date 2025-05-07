using UnityEngine;

namespace GameLogic.Grid.Area
{
    public class AreaSlot : MonoBehaviour
    {
        protected Vector3 StartPosition;
        
        public virtual void Initialize(Vector3 startPosition)
        {
            StartPosition = startPosition;
        }
        
        public virtual void OnReset()
        {
            
        }
    }
}