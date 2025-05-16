using UnityEngine;

namespace GameLogic
{
    public class AutoDestroy : MonoBehaviour
    {
        public float lifeTime = 1f;
        
        private void Start()
        {
            Destroy(gameObject, lifeTime);
        }
    }
}