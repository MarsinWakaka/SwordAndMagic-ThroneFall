// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
//
// namespace MyFramework.Utilities
// {
//     /// <summary>
//     /// 改进版数据中转站（带自动清理机制）
//     /// ❌ 不适合存储高频更新的实时数据（如每帧坐标）
//     /// ✅ 改用 事件总线 或 观察者模式 处理实时通知
//     /// </summary>
//     public static class DataBridge {
//         private static readonly Dictionary<string, (object data, bool isPersistent)> Storage = new();
//
//         // 存储临时数据（默认非持久化）
//         public static void SetTemp<T>(string key, T value) {
//             Storage[key] = (value, false);
//             
//             // 添加存储时间
//             Ages[key] = Time.time;
//         }
//
//         // 存储持久化数据
//         public static void SetPersistent<T>(string key, T value) {
//             Storage[key] = (value, true);
//         }
//
//         // 获取数据（自动清理非持久化数据）
//         public static T Get<T>(string key) {
//             if (!Storage.TryGetValue(key, out var tuple)) 
//                 throw new KeyNotFoundException(key);
//
//             var (data, isPersistent) = tuple;
//         
//             // 读取后立即清理临时数据
//             if (!isPersistent) {
//                 Storage.Remove(key);
//                 Ages.Remove(key);
//             }
//
//             return (T)data;
//         }
//
//         // 手动清理所有临时数据
//         public static void ClearAllTemp() {
//             var keysToRemove = Storage
//                 .Where(kvp => !kvp.Value.isPersistent)
//                 .Select(kvp => kvp.Key)
//                 .ToList();
//
//             foreach (var key in keysToRemove) {
//                 Storage.Remove(key);
//                 Ages.Remove(key);
//             }
//         }
//         
//         private static readonly Dictionary<string, float> Ages = new();
//         // 定期检查未清理数据
//         public static void CheckStaleData(float timeLimit = 10f) {
//             var mayExpireTime = Time.time - timeLimit;
//             var staleKeys = 
//                 Storage.Where(kvp => !kvp.Value.isPersistent && mayExpireTime > Ages[kvp.Key]); // 超过10秒未访问
//             foreach (var key in staleKeys) {
//                 Debug.LogWarning($"潜在内存泄漏: {key}");
//             }
//         }
//     }
// }