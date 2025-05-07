using System;
using System.Collections.Generic;

namespace MyFramework.Utilities
{
    // 通道类型枚举（或字符串标识）
    public enum Channel
    {
        Global,
        Gameplay,
        UI,
        Audio,
    }

    // 基础事件参数
    public interface IEventArgs { }
    
    // 事件通道类
    public class EventChannel
    {
        private readonly Dictionary<string, Delegate> _eventHandlers = new();
        
        private readonly Dictionary<Type, Delegate> Events = new();
        
        public  void Subscribe<T>(Action<T> eventHandler) where T : IEventArgs
        {
            if (Events.ContainsKey(typeof(T)))
            {
                Events[typeof(T)] = Delegate.Combine(Events[typeof(T)], eventHandler);
            }
            else
            {
                Events.Add(typeof(T), eventHandler);
            }
        }
        
        public void Unsubscribe<T>(Action<T> eventHandler) where T : IEventArgs
        {
            if (Events.ContainsKey(typeof(T)))
            {
                Events[typeof(T)] = Delegate.Remove(Events[typeof(T)], eventHandler);
                if (Events[typeof(T)] == null)
                {
                    Events.Remove(typeof(T));
                }
            }
        }
        
        public void Publish<T>(T eventArgs) where T : IEventArgs
        {
            if (Events.ContainsKey(typeof(T)))
            { 
                (Events[typeof(T)] as Action<T>)?.Invoke(eventArgs);
            }
        }
    }

    // 事件系统入口
    public static class EventBus
    {
        private static readonly Dictionary<Channel, EventChannel> Channels = new();

        public static EventChannel Channel(Channel channel)
        {
            if (!Channels.ContainsKey(channel))
            {
                Channels[channel] = new EventChannel();
            }
            return Channels[channel];
        }
    }
}