using System;
using System.Collections;
using System.Collections.Generic;
using MyFramework.Utilities.Singleton;

namespace GameLogic.TimeLine
{
    // TODO ： 修改为场景单例
    public class TimeLineManager : SceneSingleton<TimeLineManager>
    {
        private readonly Queue<Func<IEnumerator>> _timeline = new();

        protected override void OnAwake()
        {
            StartCoroutine(Execute());
        }

        protected void OnDestroy()
        {
            StopAllCoroutines();
        }

        public void AddPerform(Func<IEnumerator> perform)
        {
            _timeline.Enqueue(perform);
        }

        private IEnumerator Execute()
        {
            while (true)
            {
                if (_timeline.Count > 0)
                {
                    var timeLineEvent = _timeline.Dequeue();
                    yield return timeLineEvent();
                }
                else
                {
                    // 如果没有事件，等待一帧
                    yield return null;
                }
            }
        }
    }
}