using Unity.Netcode;
using UnityEngine;

namespace Inventory
{
    public class ItemPickup : NetworkBehaviour
    {
        [field: SerializeField]
        public InventoryItemLibrary Item { get; private set; }

        public ItemRuntimeInfo RuntimeInfo { get; set; }

        public int ItemId => Item.Index;

        public void CreateNewInfo()
        {
            if(IsServer)
            {
                RuntimeInfo = new ItemRuntimeInfo(Item.ItemName, Item.GetRandomQuality(), Item.GetRandomRarity());
                ActualizateInfoForEveryoneRPC(RuntimeInfo);
            }
        }


        [Rpc(SendTo.Everyone)]
        public void ActualizateInfoForEveryoneRPC(ItemRuntimeInfo info) => RuntimeInfo = info;

        public void Pickup(ulong clientId)
        {
            if (!IsServer) return;

            NetworkObject.Despawn(true);
        }
    }
}
