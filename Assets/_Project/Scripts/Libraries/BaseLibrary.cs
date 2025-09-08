using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


namespace Library
{
    public abstract class BaseLibrary<T> : NetworkBehaviour where T : ILibraryKey
    {
        protected static Dictionary<string, T> _dictionary = new();

        public static T GetItem(string name)
            => _dictionary.TryGetValue(name, out var item) ? item : throw new System.Exception("Error id");

        protected static void Initialize(string group)
        {
            _dictionary.Clear();

            var handle = Addressables.LoadAssetsAsync<T>(group, item =>
            {
                _dictionary.Add(item.Key, item);
            });

            handle.Completed += op =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                    Debug.Log($"Loaded {_dictionary.Count} inventory items from Addressables.");
                else
                    Debug.LogError(" Failed to load inventory items from Addressables!");
            };
        }
    }

    public interface ILibraryKey
    {
        string Key { get; }
    }
}
