using SoundSystem;
using UnityEngine;

namespace Test
{
    public class AudioTest : MonoBehaviour
    {
        [SerializeField] private string sfxID;
        [SerializeField] private string bgmID;

        [ContextMenu("PlaySFX")]
        public void PlaySFX()
        {
            SoundManager.Instance.PlaySFXOneShot(sfxID);
        }

        [ContextMenu("PlayBGM")]
        public void PlayBGM()
        {
            SoundManager.Instance.PlayBGM(bgmID);
        }
    }
}