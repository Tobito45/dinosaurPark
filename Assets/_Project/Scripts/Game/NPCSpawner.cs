using NPC;
using Unity.Netcode;
using UnityEngine;

public class NPCSpawner : NetworkBehaviour
{
    [SerializeField]
    private NetworkObject _prefab;

    [SerializeField]
    private NPCInfo[] _npcs;

    [SerializeField]
    private Transform[] _spawnPoints;

    private void Start() => SpawnNPCs();

    private void SpawnNPCs()
    {
        if (IsServer)
        {
            for (int i = 0; i < _npcs.Length; i++)
            {
                NPCInfo item = _npcs[i];
                var obj = Instantiate(_prefab, _spawnPoints[i].position, Quaternion.identity);
                obj.Spawn();
                obj.GetComponent<NPCCreator>().Init(item);
            }
        }
    }

}
