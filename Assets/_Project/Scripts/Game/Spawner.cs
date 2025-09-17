using Bootstrap;
using Inventory;
using Library;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Spawner : NetworkBehaviour, IInit
{
    [SerializeField] 
    private NetworkObject[] _itemsDinoworld, _itemsPlacement;
    [SerializeField]
    private float _minDinoworldX, _maxDinoworldX, _minDinoworldZ, _maxDinoworldZ, _height, 
            _minPlacementX, _maxPlacementX, _minPlacementZ, _maxPlacementZ;
    [SerializeField]
    private int _countDinoworld, _countPlacement;


    public void Init()
    {
        if (IsServer)
        {
            for (int i = 0; i < _countDinoworld; i++)
            {
                var item = Instantiate(SpawnItems(),
                    new Vector3(Random.Range(_minDinoworldX, _maxDinoworldX), _height, Random.Range(_minDinoworldZ, _maxDinoworldZ)), Quaternion.identity);
                item.Spawn();
                item.GetComponent<ItemPickup>().CreateNewInfo();
            }

            for (int i = 0; i < _countPlacement; i++)
            {
                var item = Instantiate(_itemsPlacement[Random.Range(0, _itemsPlacement.Count())],
                    new Vector3(Random.Range(_minPlacementX, _maxPlacementX), _height, Random.Range(_minPlacementZ, _maxPlacementZ)), Quaternion.identity);
                item.Spawn();
                item.GetComponent<ItemPickup>().CreateNewInfo();
            }
        }
    }

    private NetworkObject SpawnItems()
    {
        float totalWeight = _itemsDinoworld.Sum(i => i.GetComponent<ItemPickup>().Item.ChanceSpawn);

       float roll = UnityEngine.Random.Range(0, totalWeight);
            float cumulative = 0f;

            foreach (var item in _itemsDinoworld)
            {
                cumulative += item.GetComponent<ItemPickup>().Item.ChanceSpawn;
                if (roll <= cumulative)
                   return item;
            }

        return null;
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

        var item = InventoryItemsLibrary.GetItem(name);
        ItemRuntimeInfo info = new ItemRuntimeInfo(item.ItemName, item.GetRandomQuality(), item.GetRandomRarity());
        return GameClientsNerworkInfo.Singleton.MainPlayer.PlayerInventoryController.PutItemToList(info);
    }

}
