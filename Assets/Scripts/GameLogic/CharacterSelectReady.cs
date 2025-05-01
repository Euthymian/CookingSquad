using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterSelectReady : NetworkBehaviour
{
    public event EventHandler OnPlayerReadyChanged;

    public static CharacterSelectReady Instance { get; private set; }

    private Dictionary<ulong, bool> playersReadyDict;

    private void Awake()
    {
        Instance = this;
        playersReadyDict = new Dictionary<ulong, bool>();
    }

    public void SetPlayerReady()
    {
        SetPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        SetReadyStatusClientRpc(serverRpcParams.Receive.SenderClientId);
        playersReadyDict[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;
        foreach (ulong clientID in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playersReadyDict.ContainsKey(clientID) || !playersReadyDict[clientID])
            {
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady)
        {
            Loader.LoadNetwork(Loader.Scene.GameScene);
        }
    }

    [ClientRpc]
    private void SetReadyStatusClientRpc(ulong clientID)
    {
        playersReadyDict[clientID] = true;
        OnPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool IsPlayerReady(ulong clientID)
    {
        return playersReadyDict.ContainsKey(clientID) && playersReadyDict[clientID];
    }
}
