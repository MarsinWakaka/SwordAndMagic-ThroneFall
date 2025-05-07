// using System.Collections.Generic;
// using UnityEngine;
//
// namespace SoundSystem
// {
//     public class SoundManager : MonoBehaviour
//     {
//         // 单例实例
//         private static SoundManager Instance { get; set; }
//         
//         public enum GroupType
//         {
//             SFX,
//             BGM,
//         }
//
//         [System.Serializable]
//         public class SoundGroup
//         {
//             public GroupType groupType;
//             [Range(0, 1)] public float volume = 1f;
//             public bool mute = false;
//         }
//
//         [Header("Settings")]
//         [SerializeField] private int initialPoolSize = 5;
//         [SerializeField] private SoundGroup[] soundGroups;
//
//         private Dictionary<string, AudioClip> audioClips = new();
//         private Queue<AudioSource> availableSources = new();
//         private List<AudioSource> activeSources = new();
//         private Dictionary<GroupType, SoundGroup> groupSettings = new();
//
//         private float masterVolume = 1f;
//         private bool masterMute = false;
//
//         private void Awake()
//         {
//             if (Instance != null && Instance != this)
//             {
//                 Destroy(gameObject);
//                 return;
//             }
//
//             Instance = this;
//             DontDestroyOnLoad(gameObject);
//
//             // 初始化音效组
//             foreach (var group in soundGroups)
//             {
//                 groupSettings[group.groupType] = group;
//             }
//
//             // 创建对象池
//             for (var i = 0; i < initialPoolSize; i++)
//             {
//                 CreateNewAudioSource();
//             }
//         }
//
//         private AudioSource CreateNewAudioSource()
//         {
//             GameObject sourceObject = new GameObject("AudioSource");
//             sourceObject.transform.SetParent(transform);
//             AudioSource source = sourceObject.AddComponent<AudioSource>();
//             availableSources.Enqueue(source);
//             return source;
//         }
//
//         public void PreloadSound(string clipName)
//         {
//             if (!audioClips.ContainsKey(clipName))
//             {
//                 AudioClip clip = Resources.Load<AudioClip>($"Sounds/{clipName}");
//                 if (clip != null)
//                 {
//                     audioClips.Add(clipName, clip);
//                 }
//             }
//         }
//
//         public void PlaySound(string clipName, GroupType groupName = GroupType.SFX, float volumeScale = 1f, bool loop = false)
//         {
//             if (masterMute || !groupSettings.TryGetValue(groupName, out SoundGroup group) || group.mute)
//                 return;
//
//             if (!audioClips.TryGetValue(clipName, out AudioClip clip))
//             {
//                 clip = Resources.Load<AudioClip>($"Sounds/{clipName}");
//                 if (clip == null)
//                 {
//                     Debug.LogWarning($"Sound clip not found: {clipName}");
//                     return;
//                 }
//                 audioClips.Add(clipName, clip);
//             }
//
//             AudioSource source = GetAvailableSource();
//             source.clip = clip;
//             source.volume = Mathf.Clamp01(masterVolume * group.volume * volumeScale);
//             source.loop = loop;
//             source.Play();
//
//             if (!loop)
//             {
//                 StartCoroutine(ReturnToPoolWhenFinished(source));
//             }
//         }
//
//         private AudioSource GetAvailableSource()
//         {
//             if (availableSources.Count == 0)
//             {
//                 CreateNewAudioSource();
//             }
//
//             AudioSource source = availableSources.Dequeue();
//             activeSources.Add(source);
//             return source;
//         }
//
//         private System.Collections.IEnumerator ReturnToPoolWhenFinished(AudioSource source)
//         {
//             yield return new WaitWhile(() => source.isPlaying);
//             ReturnToPool(source);
//         }
//
//         private void ReturnToPool(AudioSource source)
//         {
//             source.Stop();
//             source.clip = null;
//             activeSources.Remove(source);
//             availableSources.Enqueue(source);
//         }
//
//         public void SetMasterVolume(float volume)
//         {
//             masterVolume = Mathf.Clamp01(volume);
//             UpdateAllVolumes();
//         }
//
//         public void SetGroupVolume(GroupType groupType, float volume)
//         {
//             if (groupSettings.TryGetValue(groupType, out SoundGroup group))
//             {
//                 group.volume = Mathf.Clamp01(volume);
//                 UpdateAllVolumes();
//             }
//         }
//
//         private void UpdateAllVolumes()
//         {
//             foreach (var source in activeSources)
//             {
//                 if (groupSettings.TryGetValue(GetGroupNameForSource(source), out SoundGroup group))
//                 {
//                     
//                 }
//             }
//         }
//
//         public void StopAllSounds()
//         {
//             foreach (var source in activeSources)
//             {
//                 ReturnToPool(source);
//             }
//         }
//
//         // 其他实用方法（停止指定音效、淡入淡出等）可以根据需要添加
//
// #if UNITY_EDITOR
//         // Editor调试信息
//         private void OnGUI()
//         {
//             if (Debug.isDebugBuild)
//             {
//                 GUI.Label(new Rect(10, 10, 200, 20), $"Active Sounds: {activeSources.Count}");
//                 GUI.Label(new Rect(10, 30, 200, 20), $"Available Sources: {availableSources.Count}");
//             }
//         }
// #endif
//     }
// }