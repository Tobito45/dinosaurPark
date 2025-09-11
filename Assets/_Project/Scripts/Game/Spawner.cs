using Library;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : NetworkBehaviour
{
    [SerializeField] 
    private NetworkObject[] _items;
    [SerializeField]
    private float _min, _max, _height;
    [SerializeField]
    private int _count;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            for (int i = 0; i < _count; i++)
            {
                var item = Instantiate(_items[Random.Range(0, _items.Count())], 
                    new Vector3(Random.Range(_min, _max), _height, Random.Range(_min, _max)), Quaternion.identity);
                item.Spawn();
            }
        }
    }

    public static bool SpawnItemOnPlayersPosition(string name)
    {
        if (!InventoryItemsLibrary.IsExistitsItem(name))
            return false;

        var camera = GameClientsNerworkInfo.Singleton.MainPlayer.MainCamere.transform;
        var spawnPos = camera.position + camera.forward * 1.2f;

        FindAnyObjectByType<Spawner>().SpawnFromServerRPC(name, spawnPos);
        return true;
    }


    [Rpc(SendTo.Server)]
    public void SpawnFromServerRPC(string itemName, Vector3 pos)
    {
        var libItem = InventoryItemsLibrary.GetItem(itemName);
        var item = Instantiate(libItem.NetworkObject, pos, Quaternion.identity);
        item.Spawn();
    }

    public static bool AddItemToPlayer(string name)
    {
        if (!InventoryItemsLibrary.IsExistitsItem(name))
            return false;

        return GameClientsNerworkInfo.Singleton.MainPlayer.PlayerInventoryController.PutItemToList(name);
    }
}
