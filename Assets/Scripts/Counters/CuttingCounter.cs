using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;

public class CuttingCounter : BaseCounter, IHasProgress
{
    public static event EventHandler OnAnyCut;
    public event EventHandler<float> OnProgessUpdate;
    public event EventHandler OnCuttingAnimationTrigger;

    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;
    private int cuttingProgress;

    new public static void ResetStaticData()
    {
        OnAnyCut = null;
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractSyncServerRpc()
    {
        InteractSyncClientRpc();
    }

    [ClientRpc]
    private void InteractSyncClientRpc()
    {
        cuttingProgress = 0;
        OnProgessUpdate?.Invoke(this, 0);
    }

    // Anytime a new object is put on the cutting counter, client will run ServerRpc which run ClientRpc to reset cuttingProgress on every player
    public override void Interact(Player player)
    {
        if (!HasKitchenObject() && player.HasKitchenObject() 
            && GetMatchedCuttingRecipe(player.GetKitchenObject().GetKitchenObjectSO()))
        {
            player.GetKitchenObject().SetKitchenObjectParent(this);

            InteractSyncServerRpc();
        }
        else if (HasKitchenObject() && !player.HasKitchenObject())
        {
            GetKitchenObject().SetKitchenObjectParent(player);
            OnProgessUpdate?.Invoke(this, 0);
        }
        else if (HasKitchenObject() && player.HasKitchenObject())
        {
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                {
                    GetKitchenObject().DestroySelf();
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void CutObjectServerRpc()
    {
        CutObjectClientRpc();
        CuttingProgressDoneServerRpc();
    }

    // every client sees the cutting progress
    [ClientRpc]
    private void CutObjectClientRpc()
    {
        cuttingProgress++;
        CuttingRecipeSO cuttingRecipeSO = GetMatchedCuttingRecipe(GetKitchenObject().GetKitchenObjectSO());
        OnProgessUpdate?.Invoke(this, (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax);
        OnCuttingAnimationTrigger?.Invoke(this, EventArgs.Empty);
        OnAnyCut?.Invoke(this, EventArgs.Empty);
    }
    
    // When cut completed, server will handle destroy original object and spawn sliced object
    [ServerRpc(RequireOwnership = false)]
    private void CuttingProgressDoneServerRpc()
    {
        CuttingRecipeSO cuttingRecipeSO = GetMatchedCuttingRecipe(GetKitchenObject().GetKitchenObjectSO());
        if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax)
        {
            KitchenObjectSO output = GetMatchedCuttingRecipe(GetKitchenObject().GetKitchenObjectSO()).output;

            KitchenObject.DestroyKitchentObject(GetKitchenObject());

            KitchenObject.SpawnKitchenObject(output, this);
        }
    }

    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject() && GetMatchedCuttingRecipe(GetKitchenObject().GetKitchenObjectSO()))
        {
            CutObjectServerRpc();
        }
    }

    private CuttingRecipeSO GetMatchedCuttingRecipe(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray)
        {
            if (cuttingRecipeSO.input == inputKitchenObjectSO)
            {
                return cuttingRecipeSO;
            }
        }
        return null;
    }
}
