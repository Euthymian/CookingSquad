using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlateCounter : BaseCounter
{
    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemoved;

    private float spawnTimer;
    private float spawnTimerMax = 4;
    private int platesSpawnedAmount;
    private int platesSpawnedAmountMax = 4;

    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;

    private void Update()
    {
        if(!IsServer) return;

        spawnTimer += Time.deltaTime;
        if(spawnTimer > spawnTimerMax)
        {
            spawnTimer = 0;

            if(GameManager.Instance.IsGamePlaying() && platesSpawnedAmount < platesSpawnedAmountMax)
            {
                SpawnPlatesServerRpc();
            }
        }
    }

    [ServerRpc]
    private void SpawnPlatesServerRpc()
    {
        SpawnPlatesClientRpc();
    }

    [ClientRpc]
    private void SpawnPlatesClientRpc()
    {
        platesSpawnedAmount++;
        OnPlateSpawned?.Invoke(this, EventArgs.Empty);
    }

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            if (platesSpawnedAmount > 0)
            {
                KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);
                InteractSyncServerRpc();
            }
        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void InteractSyncServerRpc()
    {
        InteractSyncClientRpc();
    }

    [ClientRpc]
    private void InteractSyncClientRpc()
    {
        platesSpawnedAmount--;
        OnPlateRemoved?.Invoke(this, EventArgs.Empty);
    }
}
