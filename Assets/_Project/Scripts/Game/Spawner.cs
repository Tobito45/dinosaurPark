using System.Linq;
using Unity.Netcode;
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
}
