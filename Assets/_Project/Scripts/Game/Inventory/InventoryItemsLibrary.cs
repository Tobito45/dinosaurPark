using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class InventoryItemsLibrary : NetworkBehaviour
{
    [SerializeField]
    private List<InventoryItemLibrary> _list = new();

    private static Dictionary<int, InventoryItemLibrary> _dictionary = new();

    private void Start()
    {
        if (!IsOwner)
            return;

        _dictionary.Clear();
        foreach (var item in _list)
            _dictionary.Add(item.Index, item);
    }

    public static InventoryItemLibrary GetIInventoryItem(int id) => _dictionary.TryGetValue(id, out var item) ? item : throw new System.Exception("Error id");
}
