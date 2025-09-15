using Bootstrap;
using GameUI;
using NPC;
using Unity.Netcode;
using UnityEngine;

public class NPCSpawner : NetworkBehaviour, IInit
{
    [Header("References")]
    [SerializeField]
    private StatisticsUI _statisticsUI;


    [SerializeField]
    private NetworkObject _prefab;

    [SerializeField]
    private NPCInfo[] _npcs;

    [SerializeField]
    private Transform[] _spawnPoints;


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
                obj.GetComponent<NPCController>().AddOnEmothion(_statisticsUI.OnNewEmothion, _statisticsUI.OnStartWatching);
            }
        }
    }

    public void Init() => SpawnNPCs();
}
