using GameLogic.BUFF;
using UnityEngine;

namespace Test
{
    public class SerializationTest : MonoBehaviour
    {
        [SerializeField] private Buff buff;
        
        private void Start()
        {
            // 测试 BuffInstance 的序列化
            // var buffIns = new CoverInstance("Cover");
            // Debug.Log(JsonUtility.ToJson(buffIns));
            // Debug.Log(JsonUtility.ToJson(buffIns));
            
        }
    }
}