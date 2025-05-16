using MyFramework.Utilities.Singleton;
using UnityEngine;

namespace GameLogic
{
    public enum VFXType
    {
        None,
        Attack,
        Move,
        Skill,
        Death,
        Hit,
        Heal,
        Buff,
        DeBuff,
        Spawn,
        Despawn,
        Teleport,
        Summon,
        Destroy
    }
    
    public class VFXManager : MonoSingleton<VFXManager>
    {
        // 简单实现
        public GameObject vfxPrefab;

        public GameObject GetFX()
        {
            return Instantiate(vfxPrefab, transform);
        }
    }
}