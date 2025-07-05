using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharactersController : MonoBehaviour
{
    [SerializeField] 
    private GameObject _gamePlayerPrefab;
    
    private Dictionary<ulong, GameObject> _playerObjects = new Dictionary<ulong, GameObject>();

    private void Start()
    {
        if (NetworkManager.Singleton.IsHost)
            foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
               SpawnGamePlayerForClient(clientId);
    }

    private void SpawnGamePlayerForClient(ulong clientId)
    {
        var playerGO = Instantiate(_gamePlayerPrefab);
        var networkObject = playerGO.GetComponent<NetworkObject>();

        playerGO.transform.position = new Vector3(
            Random.Range(-5f, 5f),
            1f,
            Random.Range(-5f, 5f)
        );

        Debug.Log(NetworkManager.Singleton.IsHost);
        networkObject.SpawnAsPlayerObject(clientId);
        
        _playerObjects[clientId] = playerGO;
    }
}
