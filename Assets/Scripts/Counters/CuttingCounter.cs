using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CuttingCounter : BaseCounter, IHasProgress
{
    public static event EventHandler OnAnyCut;
    public event EventHandler<float> OnProgessUpdate;
    public event EventHandler OnCuttingAnimationTrigger;

    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;
    private int cuttingProgress;

    public override void Interact(Player player)
    {
        if (!HasKitchenObject() && player.HasKitchenObject() 
            && GetMatchedCuttingRecipe(player.GetKitchenObject().GetKitchenObjectSO()))
        {
            player.GetKitchenObject().SetKitchenObjectParent(this);
            cuttingProgress = 0;
            OnProgessUpdate?.Invoke(this, 0);
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

    public override void InteractAlternate(Player player)
    {
        if (HasKitchenObject() && GetMatchedCuttingRecipe(GetKitchenObject().GetKitchenObjectSO()))
        {
            cuttingProgress++;
            CuttingRecipeSO cuttingRecipeSO = GetMatchedCuttingRecipe(GetKitchenObject().GetKitchenObjectSO());
            OnProgessUpdate?.Invoke(this, (float)cuttingProgress/cuttingRecipeSO.cuttingProgressMax);
            OnCuttingAnimationTrigger?.Invoke(this, EventArgs.Empty);
            OnAnyCut?.Invoke(this, EventArgs.Empty);

            if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax)
            {
                KitchenObjectSO output = GetMatchedCuttingRecipe(GetKitchenObject().GetKitchenObjectSO()).output;

                GetKitchenObject().DestroySelf();

                KitchenObject.SpawnKitchenObject(output, this);
            }
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
