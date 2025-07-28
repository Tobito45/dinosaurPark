using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
namespace Character
{
    public class CharactersController : MonoBehaviour
    {
        [SerializeField]
        private GameObject _gamePlayerPrefab;
        [SerializeField]
        private Transform _spawnPoint;
        [SerializeField]
        private Vector2 _spawnRange; //min and max

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

            playerGO.transform.position = _spawnPoint.position + new Vector3(
                Random.Range(_spawnRange.x,_spawnRange.y),
                1f,
                Random.Range(_spawnRange.x, _spawnRange.y)
            );

            Debug.Log(NetworkManager.Singleton.IsHost);
            networkObject.SpawnAsPlayerObject(clientId);

            _playerObjects[clientId] = playerGO;
        }
    }
}
