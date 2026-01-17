using System;
using System.Linq;
using System.Reflection;
using UnityEngine;


namespace DI
{
    public static class DIExtensions
    {
        private static DIContainer _global;
        
        public static void InitGlobal(this DIContainer container) => _global = container;

        public static void RegisterAllWithPriority(this DIContainer container)
        {
            var prioritized = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass &&
                            !t.IsAbstract &&
                            typeof(MonoBehaviour).IsAssignableFrom(t) &&
                            t.GetCustomAttribute<PriorityAttribute>() != null)
                .Select(t => new
                {
                    Type = t,
                    Priority = t.GetCustomAttribute<PriorityAttribute>()!.Value,
                    Instance = UnityEngine.Object.FindFirstObjectByType(t)
                })
                .Where(x => x.Instance != null)
                .OrderByDescending(x => x.Priority);

            foreach (var item in prioritized)
            {
                container.Bind(item.Type, item.Instance);
                Debug.Log($"[DI] registred with [{item.Priority}]: {item.Type.Name}");
            }
        }

        public static void Inject(this MonoBehaviour mb)
        {
            var fields = mb.GetType().GetFields(System.Reflection.BindingFlags.Instance
                                               | System.Reflection.BindingFlags.NonPublic
                                               | System.Reflection.BindingFlags.Public);

            foreach (var field in fields)
            {
                if (field.GetCustomAttributes(typeof(InjectAttribute), false).Length > 0)
                {
                    var value = _global.Resolve(field.FieldType);
                    field.SetValue(mb, value);
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class InjectAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class PriorityAttribute : Attribute
    {
        public int Value { get; }
        public PriorityAttribute(int priority = 0) => Value = priority;
    }

}
