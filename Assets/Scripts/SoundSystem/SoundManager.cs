using System;
using System.Collections.Generic;
using MyFramework.Utilities.Singleton;
using UnityEngine;

namespace SoundSystem
{
    public class SoundManager : MonoSingleton<SoundManager>
    {
        [Serializable]
        public struct AudioData
        {
            public AudioClip clip;
            public float volume;
        }

        private AudioSource bgmSource;
        private AudioSource sfxSource;
        
        public float BGMVolume
        {
            get => bgmSource.volume;
            set => bgmSource.volume = value;
        }
        
        public float SFXVolume
        {
            get => sfxSource.volume;
            set => sfxSource.volume = value;
        }

        protected override void WhenAwake()
        {
            base.WhenAwake();
            bgmSource = gameObject.AddComponent<AudioSource>();
            sfxSource = gameObject.AddComponent<AudioSource>();
        }
        
        public void PlaySFXOneShot(AudioClip clip, float volume = 1f)
        {
            if (clip == null)
            {
                Debug.LogWarning("Audio clip is null.");
                return;
            }

            sfxSource.PlayOneShot(clip, volume);
        }
        
        public void PlayBGM(AudioClip clip, float volume = 1f)
        {
            if (clip == null)
            {
                Debug.LogWarning($"Audio clip is null.");
                return;
            }

            bgmSource.clip = clip;
            bgmSource.volume = volume;
            bgmSource.loop = true;
            bgmSource.Play();
        }

        public void PlaySFXOneShot(AudioData audioData)
        {
            PlaySFXOneShot(audioData.clip, audioData.volume);
        }
        
        public void PlayBGM(AudioData audioData)
        {
            PlayBGM(audioData.clip, audioData.volume);
        }

        #region Play By AudioID
        
        // 替换为LRU
        private readonly Dictionary<string, AudioClip> lookUpCache = new();
        private const string SFXPath = "Sounds/SFX/";
        private const string BGMPath = "Sounds/BGM/";

        private AudioClip GetAudioClip(string audioID, bool isSFX = true)
        {
            if (lookUpCache.TryGetValue(audioID, out var clip))
            {
                return clip;
            }

            // 这里可以添加从资源加载音频的逻辑
            var fullPath = (isSFX ? SFXPath : BGMPath) + audioID;
            clip = Resources.Load<AudioClip>(fullPath);
            
            lookUpCache[audioID] = clip;
            return clip;
        }
        
        public void PlaySFXOneShot(string audioID, float volume = 1f)
        {
            var clip = GetAudioClip(audioID);
            PlaySFXOneShot(clip, volume);
        }
        
        public void PlayBGM(string audioID, float volume = 1f)
        {
            var clip = GetAudioClip(audioID, false);
            PlayBGM(clip, volume);
        }

        #endregion
    }
}