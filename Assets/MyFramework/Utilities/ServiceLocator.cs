using System;
using System.Collections.Generic;

namespace MyFramework.Utilities
{
    public interface IServiceProvider
    {
    }

    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, IServiceProvider> Services = new();

        public static void Register<T>(T service) where T : IServiceProvider
        {
            if (Services.ContainsKey(typeof(T)))
            {
                throw new InvalidOperationException($"Service of type {typeof(T)} already registered.");
            }

            Services[typeof(T)] = service;
        }
        
        public static void UnRegister<T>() where T : IServiceProvider
        {
            if (!Services.ContainsKey(typeof(T)))
            {
                throw new InvalidOperationException($"Service of type {typeof(T)} not found.");
            }

            Services.Remove(typeof(T));
        }

        public static T Resolve<T>() where T : IServiceProvider
        {
            if (!Services.TryGetValue(typeof(T), out var service))
            {
                throw new InvalidOperationException($"Service of type {typeof(T)} not found.");
            }

            return (T)service;
        }
    }
}