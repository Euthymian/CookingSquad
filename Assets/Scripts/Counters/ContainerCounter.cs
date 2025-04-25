using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public event EventHandler OnPlayerInteractAnimationTrigger;

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            KitchenObject.SpawnKitchenObject(kitchenObjectSO,player); // <- Go to definition
            HandleAnimationSyncServerRpc();
        }
    }

    // The same as DeliveryManager. Animation of container counter need to be trggered at every player. Players trggier animation 
    // -> a player runs ServerRpc, ServerRpc runs ClientRpc

    [ServerRpc(RequireOwnership = false)]
    private void HandleAnimationSyncServerRpc()
    {
        HandleAnimationSyncClientRpc();
    }

    [ClientRpc]
    private void HandleAnimationSyncClientRpc()
    {
        OnPlayerInteractAnimationTrigger?.Invoke(this, EventArgs.Empty);
    }
}
