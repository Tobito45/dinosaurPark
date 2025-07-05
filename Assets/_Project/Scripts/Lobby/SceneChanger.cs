using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : NetworkBehaviour
{
    [Header("Game Settings")]
    [SerializeField] 
    private string _gameSceneName = "GameScene";

    private static SceneChanger _instance;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void StartGame()
    {
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnSceneLoadCompleted;

        StartGameServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    void StartGameServerRpc()
    {
        var status = NetworkManager.Singleton.SceneManager.LoadScene(_gameSceneName, LoadSceneMode.Single);
    }

    void OnSceneLoadCompleted(string sceneName, LoadSceneMode mode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (sceneName == _gameSceneName)
        {
            NotifyGameStartClientRpc();

            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= OnSceneLoadCompleted;
        }
    }

    [ClientRpc]
    void NotifyGameStartClientRpc()
    {
        Debug.Log("Game Started!");
       
    }
}
