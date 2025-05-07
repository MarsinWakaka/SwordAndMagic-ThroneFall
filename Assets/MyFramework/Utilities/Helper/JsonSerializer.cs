using UnityEngine;

namespace MyFramework.Utilities.Helper
{
    public interface IJsonSerializer
    {
        string Serialize<T>(T obj);
        T Deserialize<T>(string json);
    }
    
    public class JsonSerializer : IJsonSerializer
    {
        public string Serialize<T>(T obj)
        {
            // TODO 实现序列化逻辑
            return JsonUtility.ToJson(obj);
        }

        public T Deserialize<T>(string json)
        {
            // TODO 实现反序列化逻辑
            return JsonUtility.FromJson<T>(json);
        }
    }
}