using System.Collections;
using System.Collections.Generic;
using GameLogic.Unit.Controller;
using UnityEngine;

namespace GameLogic.TimeLine
{
    public class MoveWrapper : IPerform
    {
        public MoveWrapper(CharacterUnitController unit, List<Vector2Int> path)
        {
            Path = path;
            Unit = unit;
        }

        private List<Vector2Int> Path { get; }
        private CharacterUnitController Unit { get; }
        
        public IEnumerator Perform()
        {
            // TODO : 处理移动逻辑
            // TODO : 播放移动动画
            // TODO : 播放移动音效
            // TODO : 播放移动特效
            // TODO : 播放移动粒子特效
            // TODO : 播放移动粒子特效
            // TODO : 播放移动粒子特效
            // TODO : 播放移动粒子特效
            
            foreach (var pos in Path)
            {
                Unit.Teleport(pos);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}