using System;
using System.Collections;
using System.Collections.Generic;
using MyFramework.Utilities.Singleton;

namespace GameLogic.TimeLine
{
    public interface IPerform
    {
        IEnumerator Perform();
    }
    
    // TODO ： 修改为场景单例
    public class TimeLineManager : SceneSingleton<TimeLineManager>
    {
        private readonly Queue<Func<IEnumerator>> _timeline = new();
        
        public bool IsRunning => _timeline.Count > 0;

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
        
        public void AddPerform(IPerform perform)
        {
            _timeline.Enqueue(perform.Perform);
        }

        private IEnumerator Execute()
        {
            while (true)
            {
                if (_timeline.Count > 0)
                {
                    var timeLineEvent = _timeline.Peek();
                    yield return timeLineEvent();
                    _timeline.Dequeue();
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