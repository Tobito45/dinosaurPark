using Dinosaurus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;


namespace DI
{
    public class DIContainer
    {
        private readonly Dictionary<Type, object> _instances = new();
        private readonly Dictionary<Type, Func<object>> _factories = new(); //simple factory
        public readonly Dictionary<DinoType, object> _dinos = new();

        // reg as Singleton
        public void Bind<T>(T instance) where T : class
        {
            Debug.Log($"[DI] Binding instance of type {typeof(T)} {instance}");
            _instances[typeof(T)] = instance;
        }

        // reg as with factory
        public void Bind<T>(Func<T> factory) where T : class => _factories[typeof(T)] = () => factory();

        //lazy instance
        // reg as Singleton with factory
        public void BindSingleton<T>(Func<T> factory) where T : class
        {
            var lazy = new Lazy<T>(factory);
            _factories[typeof(T)] = () => lazy.Value;
        }
        public void Bind(Type type, object instance) => _instances[type] = instance;
        public void Bind(DinoType type, object instance) => _dinos[type] = instance;

        // get instance
        public T Resolve<T>() where T : class
        {
            Type type = typeof(T);

            if (_instances.TryGetValue(type, out var instance))
                return (T)instance;

            if (_factories.TryGetValue(type, out var factory))
                return (T)factory();

            throw new Exception($"Type {type} are not present in container");
        }
        public object Resolve(Type type)
        {
            if (_instances.TryGetValue(type, out var instance))
                return instance;

            if (_factories.TryGetValue(type, out var factory))
                return factory();

            throw new Exception($"Type {type} are not present in container");
        }
        public object Resolve(DinoType dino)
        {
            if (_dinos.TryGetValue(dino, out var instance))
                return instance;

            throw new Exception($"Type {dino} are not present in container");
        }

    }
}
