using Library;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Library
{
    public class InventoryItemsLibrary : BaseLibrary<InventoryItemLibrary>
    {
        private void Start() => Initialize("item");
    }
}
