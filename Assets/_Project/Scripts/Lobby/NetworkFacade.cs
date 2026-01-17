using System;
using UnityEngine;

public class NetworkFacade
{
    private INetworkService _network;

    public NetworkFacade(INetworkService network) => _network = network;

    public void StartHost(int players) => _network.StartHost(players);
    public void JoinGame(ulong hostId) => _network.JoinGame(hostId);
    public void LeaveGame() => _network.LeaveGame();

    public bool IsHost => _network.IsHost;
    public bool IsConnected => _network.IsConnected;
    public ulong LocalClientId => _network.LocalClientId;

    public event Action OnConnected
    {
        add => _network.OnConnected += value;
        remove => _network.OnConnected -= value;
    }
    public event Action<ulong> OnPlayerJoined
    {
        add => _network.OnPlayerJoined += value;
        remove => _network.OnPlayerJoined -= value;
    }

    public event Action<ulong> OnPlayerLeft
    {
        add => _network.OnPlayerLeft += value;
        remove => _network.OnPlayerLeft -= value;
    }
    public event Action OnLobbyCreated
    {
        add => _network.OnLobbyCreated += value;
        remove => _network.OnLobbyCreated -= value;
    }
    public event Action<string> OnError
    {
        add => _network.OnError += value;
        remove => _network.OnError -= value;
    }
    public event Action OnDisconnected
    {
        add => _network.OnDisconnected += value;
        remove => _network.OnDisconnected -= value;
    }
}
