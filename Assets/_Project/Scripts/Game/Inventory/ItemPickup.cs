using Unity.Netcode;
using UnityEngine;

namespace Inventory
{
    public class ItemPickup : NetworkBehaviour
    {
        [field: SerializeField]
        public int ItemId { get; private set; }
        public void Pickup(ulong clientId)
        {
            if (!IsServer) return;

            //var player = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
            //var inventory = player.GetComponent<NetworkInventory>();
            //inventory.AddItemServerRpc(itemId, amount);

            NetworkObject.Despawn(true);
        }
    }
}
