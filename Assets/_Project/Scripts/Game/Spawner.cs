using Unity.Netcode;
using UnityEngine;

public class Spawner : NetworkBehaviour
{
    [SerializeField] private NetworkObject itemPrefab, itemCube, itemSphere;
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            var item = Instantiate(itemPrefab, new Vector3(Random.Range(-3, 3),1, Random.Range(-3, 3)), Quaternion.identity);
            item.Spawn();

            var item2 = Instantiate(itemCube, new Vector3(Random.Range(-3, 3), 1, Random.Range(-3, 3)), Quaternion.identity);
            item2.Spawn();

            var item3 = Instantiate(itemSphere, new Vector3(Random.Range(-3, 3), 1, Random.Range(-3, 3)), Quaternion.identity);
            item3.Spawn();
        }
    }
}
