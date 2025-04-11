using System;
using System.Collections;
using System.Collections.Generic;
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
        spawnTimer += Time.deltaTime;
        if(spawnTimer > spawnTimerMax)
        {
            spawnTimer = 0;

            if(GameManager.Instance.IsGamePlaying() && platesSpawnedAmount < platesSpawnedAmountMax)
            {
                platesSpawnedAmount++;
                OnPlateSpawned?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            if (platesSpawnedAmount > 0)
            {
                platesSpawnedAmount--;
                KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);
                OnPlateRemoved?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
