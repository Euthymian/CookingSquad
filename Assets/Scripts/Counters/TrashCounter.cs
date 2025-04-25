using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TrashCounter : BaseCounter
{
    public static event EventHandler OnDestroyObject;

    new public static void ResetStaticData()
    {
        OnDestroyObject = null;
    }

    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            KitchenObject.DestroyKitchentObject(player.GetKitchenObject());
            InteractSyncServerRpc();
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
        OnDestroyObject?.Invoke(this, EventArgs.Empty);
    }
}
