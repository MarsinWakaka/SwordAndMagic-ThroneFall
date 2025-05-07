using System;
using System.Collections.Generic;

namespace MyFramework.Utilities
{
    public static class EventCenter
    {
        private class WeakReference<T> where T : class
        {
            
        }
        
        public static void RegisterWeak<T>(Action<T> eventHandler) where T : IEventArgs
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
        
        private static readonly Dictionary<Type, Delegate> Events = new();
        
        public static void Register<T>(Action<T> eventHandler) where T : IEventArgs
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
        
        public static void Unregister<T>(Action<T> eventHandler) where T : IEventArgs
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
        
        public static void Publish<T>(T eventArgs) where T : IEventArgs
        {
            if (Events.ContainsKey(typeof(T)))
            { 
                (Events[typeof(T)] as Action<T>)?.Invoke(eventArgs);
            }
        }
    }
}