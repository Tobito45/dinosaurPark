using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Netcode;
using Unity.VisualScripting;

public class GameClientsNerworkInfo : NetworkBehaviour
{
    private Dictionary<ulong, (string name, ulong id)> _playersInfo = new();
    public static GameClientsNerworkInfo Singleton { get; private set; }
    private void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Destroy(gameObject);
            return;
        }

        Singleton = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddPlayer(ulong id, string name, ulong idSteam) =>
        _playersInfo[id] = (name, idSteam);

    public (string name, string idSteam) GetPlayer(ulong id)
    {
        if (_playersInfo.TryGetValue(id, out var playerInfo))
            return (playerInfo.name, playerInfo.id.ToString());
        return (string.Empty, string.Empty);
    }

    [Rpc(SendTo.Everyone)]
    public void AddPlayerRpc(ulong id, string name, ulong steamId) => AddPlayer(id, name, steamId);

    public override string ToString()
    {
        string result = "Players Info:\n";
        foreach (var player in _playersInfo)
        {
            result += $"ID: {player.Key}, Name: {player.Value.name}, Steam ID: {player.Value.id}\n";
        }
        return result;
    }

}
