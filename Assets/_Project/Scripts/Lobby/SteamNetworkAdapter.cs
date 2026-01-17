using UnityEngine;
using Unity.Netcode;
using Steamworks;
using Steamworks.Data;
using Netcode.Transports.Facepunch;
using System;
using System.Collections.Generic;

public class SteamNetworkAdapter : MonoBehaviour, INetworkService
{
    private FacepunchTransport _transport = null;
    public Lobby? CurrentLobby { get; private set; } = null;
    
    public bool IsHost => NetworkManager.Singleton.IsHost;
    public bool IsConnected => NetworkManager.Singleton.IsConnectedClient || IsHost;
    public ulong LocalClientId => NetworkManager.Singleton.LocalClientId;

    public ulong hostId;

    public event Action OnConnected;
    public event Action<ulong> OnPlayerJoined;
    public event Action<ulong> OnPlayerLeft;
    public event Action OnLobbyCreated;
    public event Action<string> OnError;
    public event Action OnDisconnected;

    private void Start()
    {
        _transport = GetComponent<FacepunchTransport>();

        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        SteamMatchmaking.OnLobbyCreated += SteamMatchmaking_OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered += SteamMatchmaking_OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined += SteamMatchmaking_OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave += SteamMatchmaking_OnLobbyMemberLeave;
        SteamMatchmaking.OnLobbyInvite += SteamMatchmaking_OnLobbyInvite;
        SteamMatchmaking.OnLobbyGameCreated += SteamMatchmaking_OnLobbyGameCreated;
        SteamFriends.OnGameLobbyJoinRequested += SteamFriends_OnGameLobbyJoinRequested;
    }

    private void OnDestroy()
    {
        SteamMatchmaking.OnLobbyCreated -= SteamMatchmaking_OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered -= SteamMatchmaking_OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined -= SteamMatchmaking_OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave -= SteamMatchmaking_OnLobbyMemberLeave;
        SteamMatchmaking.OnLobbyInvite -= SteamMatchmaking_OnLobbyInvite;
        SteamMatchmaking.OnLobbyGameCreated -= SteamMatchmaking_OnLobbyGameCreated;
        SteamFriends.OnGameLobbyJoinRequested -= SteamFriends_OnGameLobbyJoinRequested;

        if (NetworkManager.Singleton == null)
            return;
    
        NetworkManager.Singleton.OnServerStarted -= Singlton_OnServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback -= Singlton_OnClientConnectedCallback; 
        NetworkManager.Singleton.OnClientDisconnectCallback -= Singleton_OnClientDisconnectCallback;
    }


    private void OnApplicationQuit()
    {
        Disconnected();
    }

    //when you accept the invite or Join on a friend
    private async void SteamFriends_OnGameLobbyJoinRequested(Lobby lobby, SteamId id)
    {
        RoomEnter joinedLobby = await lobby.Join();
        if(joinedLobby != RoomEnter.Success)
        {
            Debug.LogError($"Failed to join lobby: {joinedLobby}");
            OnError?.Invoke($"Failed to join lobby: {joinedLobby}");
        }
        else
        {
            OnConnected?.Invoke();
            CurrentLobby = lobby;
            Debug.Log($"Joined lobby: {lobby.Id}");
            GameManager.instance.SendMessageToChat($"Joined lobby: {lobby.Id}", NetworkManager.Singleton.LocalClientId, true);
        }

    }

    private void SteamMatchmaking_OnLobbyGameCreated(Lobby lobby, uint ip, ushort port, SteamId id)
    {
        Debug.Log("Lobby was created");
        GameManager.instance.SendMessageToChat($"Lobby was created", NetworkManager.Singleton.LocalClientId, true);

    }


    //friend send you an steam invite
    private void SteamMatchmaking_OnLobbyInvite(Friend friend, Lobby lobby)
    {
        Debug.Log($"Invite from {friend.Name}"); 
    }

    private void SteamMatchmaking_OnLobbyMemberLeave(Lobby lobby, Friend friend)
    {
        Debug.Log($"Member {friend.Name} left the lobby");
        GameManager.instance.SendMessageToChat($"{friend.Name} has left", friend.Id, true);
        NetworkTransmission.instance.RemoveMeFromDictionaryServerRPC(friend.Id);
        OnPlayerLeft?.Invoke(friend.Id);
    }

    private void SteamMatchmaking_OnLobbyMemberJoined(Lobby lobby, Friend friend)
    {
        Debug.Log($"Member {friend.Name} {friend.Id} joined the lobby");
        GameManager.instance.SendMessageToChat($"Member {friend.Name} joined the lobby", NetworkManager.Singleton.LocalClientId, true);
        OnPlayerJoined?.Invoke(friend.Id);
    }

    private void SteamMatchmaking_OnLobbyEntered(Lobby lobby)
    {
        if (NetworkManager.Singleton.IsHost)
            return;

        StartClient(CurrentLobby.Value.Owner.Id);
        GameManager.instance.SendMessageToChat($"Joined lobby: {lobby.Id}", NetworkManager.Singleton.LocalClientId, true);
        OnConnected?.Invoke();
    }

    private void SteamMatchmaking_OnLobbyCreated(Result result, Lobby lobby)
    {
        if(result != Result.OK)
        {
            Debug.LogError($"Failed to create lobby: {result}");
            OnError?.Invoke($"Failed to join lobby: {result}");

            return;
        }

        lobby.SetPublic();
        lobby.SetJoinable(true);
        lobby.SetGameServer(lobby.Owner.Id);
        Debug.Log($"Lobby created: {lobby.Id}");
        NetworkTransmission.instance.AddMeToDictionaryServerRPC(SteamClient.SteamId, SteamClient.Name, NetworkManager.Singleton.LocalClientId);
        OnLobbyCreated?.Invoke();   
    }

    public async void StartHost(int maxMembers)
    {
        NetworkManager.Singleton.OnServerStarted += Singlton_OnServerStarted;
        NetworkManager.Singleton.StartHost();
        GameManager.instance.myClientId = NetworkManager.Singleton.LocalClientId;
        CurrentLobby = await SteamMatchmaking.CreateLobbyAsync(maxMembers);
        GameClientsNerworkInfo.Singleton.AddPlayer(NetworkManager.Singleton.LocalClientId, SteamClient.Name, SteamClient.SteamId);
    }

    public void StartClient(SteamId sId)
    {
        NetworkManager.Singleton.OnClientConnectedCallback += Singlton_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientDisconnectCallback += Singleton_OnClientDisconnectCallback;
        _transport.targetSteamId = sId;
        GameManager.instance.myClientId = NetworkManager.Singleton.LocalClientId;
        GameClientsNerworkInfo.Singleton.AddPlayer(NetworkManager.Singleton.LocalClientId, SteamClient.Name, SteamClient.SteamId);
        if (NetworkManager.Singleton.StartClient())
            Debug.Log("Client started");
        else
        {
            OnError?.Invoke($"Failed to start client");
            Debug.LogError("Failed to start client");
        }
    }

    public void Disconnected()
    {
        CurrentLobby?.Leave();
        if(NetworkManager.Singleton == null)
            return;

        if(NetworkManager.Singleton.IsHost)
            NetworkManager.Singleton.OnServerStarted -= Singlton_OnServerStarted;
        else
            NetworkManager.Singleton.OnClientConnectedCallback -= Singlton_OnClientConnectedCallback;

        NetworkManager.Singleton.Shutdown(true);
        GameManager.instance.ClearChat();
        OnDisconnected?.Invoke();
        Debug.Log("Disconnected");
    }

    private void Singleton_OnClientDisconnectCallback(ulong clientId)
    {
        NetworkManager.Singleton.OnClientDisconnectCallback -= Singleton_OnClientDisconnectCallback;
        if (clientId == 0)
            Disconnected();
    }

    private void Singlton_OnClientConnectedCallback(ulong clientId)
    {
        NetworkTransmission.instance.AddMeToDictionaryServerRPC(SteamClient.SteamId, SteamClient.Name, clientId);
        GameManager.instance.myClientId = clientId;
        NetworkTransmission.instance.IsTheClientReadyServerRPC(false, clientId);
        Debug.Log($"Client has connected : {clientId}");
        OnPlayerJoined?.Invoke(clientId);
    }

    private void Singlton_OnServerStarted()
    {
        Debug.Log("Host started");
        OnLobbyCreated?.Invoke();
    }

    public void JoinGame(ulong hostSteamId)
    {
        _transport.targetSteamId = new SteamId { Value = hostSteamId };
        NetworkManager.Singleton.StartClient();
    }

    public void LeaveGame()
    {
        CurrentLobby?.Leave();
        NetworkManager.Singleton.Shutdown();
        OnPlayerLeft?.Invoke(LocalClientId);
    }
}