using Unity.Netcode;
using UnityEngine;

namespace Inventory
{
    public class ItemPickup : NetworkBehaviour
    {
        [field: SerializeField]
        public InventoryItemLibrary Item { get; private set; }

        public int ItemId => Item.Index;
        public void Pickup(ulong clientId)
        {
            if (!IsServer) return;

            NetworkObject.Despawn(true);
        }
    }
}
