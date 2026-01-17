using System;
using System.Runtime.InteropServices;
using UnityEngine;

public interface INetworkService
{
    void StartHost(int maxPlayers);
    void JoinGame(ulong hostSteamId);
    void LeaveGame();

    bool IsHost { get; }
    bool IsConnected { get; }
    ulong LocalClientId { get; }

    event Action OnConnected;
    event Action<ulong> OnPlayerJoined;
    event Action<ulong> OnPlayerLeft;
    event Action OnLobbyCreated;
    event Action<string> OnError;
    event Action OnDisconnected;

}
