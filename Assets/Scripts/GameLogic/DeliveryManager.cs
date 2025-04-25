using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour
{
    public event EventHandler OnUpdateWaitingRecipesList;
    public event EventHandler OnDeliverRecipeSuccess;
    public event EventHandler OnDeliverRecipeFail;

    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private RecipeListSO availableRecipeSO;

    private List<RecipeSO> waitingRecipeSOList;
    private float spawnRecipeTimer;
    private float spawnRecipeTimerMax = 4;
    private int maxRecipes = 4;
    private int numRecipesDelivered = 0;

    private void Awake()
    {
        Instance = this;

        waitingRecipeSOList = new List<RecipeSO>();
    }

    private void Update()
    {
        if (!IsServer) return;

        spawnRecipeTimer -= Time.deltaTime;
        if (spawnRecipeTimer <= 0) {
            spawnRecipeTimer = spawnRecipeTimerMax;
            
            if(GameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < maxRecipes)
            {
                int newWaitingRecipeIndex = UnityEngine.Random.Range(0, availableRecipeSO.recipeSOList.Count);
                SendRecipeToClientRpc(newWaitingRecipeIndex);
            }
        }
    }
    /*
        Only server generate new waiting recipe
        then the ClientRpc will notify all client to update the WaitingRecipesUI 
     */
    [ClientRpc]
    private void SendRecipeToClientRpc(int newWaitingRecipeIndex)
    {
        RecipeSO newWaitingRecipe = availableRecipeSO.recipeSOList[newWaitingRecipeIndex];
        waitingRecipeSOList.Add(newWaitingRecipe);

        OnUpdateWaitingRecipesList?.Invoke(this, EventArgs.Empty);
    }

    public List<RecipeSO> GetWaitingRecipesSOList()
    {
        return waitingRecipeSOList;
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject)
    {
        for (int i = 0; i < waitingRecipeSOList.Count; i++)
        {
            RecipeSO iRecipe = waitingRecipeSOList[i];

            if (iRecipe.kitchenObjectSOList.Count == plateKitchenObject.GetListKitchenObjectList().Count)
            {
                bool plateContentMatchesRecipe = true;
                foreach (KitchenObjectSO recipeKitchenObjectSO in iRecipe.kitchenObjectSOList)
                {
                    bool ingredientFound = false;
                    foreach (KitchenObjectSO plateKitchenObjectSO in plateKitchenObject.GetListKitchenObjectList())
                    {
                        if (recipeKitchenObjectSO == plateKitchenObjectSO)
                        {
                            ingredientFound = true;
                            break;
                        }
                    }
                    if (!ingredientFound)
                    {
                        plateContentMatchesRecipe = false;
                        break;
                    }
                }

                if (plateContentMatchesRecipe)
                {
                    DeliverCorrectRecipeServerRpc(i);

                    return;
                }
            }
        }

        DeliverIncorrectRecipeServerRpc();
    }

    /*
        This is client authoritative system. When deliver, client will run the ServerRpc (set RequireOwnership = False -> Any clients can run ServerRpc)
        The ServerRpc will then run ClientRpc that notifies every player to popup delivery animation and update RecipesWaiting if the delivery is correct
     */

    [ServerRpc(RequireOwnership = false)]
    private void DeliverIncorrectRecipeServerRpc()
    {
        DeliverIncorrectRecipeClientRpc();
    }

    [ClientRpc]
    private void DeliverIncorrectRecipeClientRpc()
    {
        OnDeliverRecipeFail?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverCorrectRecipeServerRpc(int currentRecipeSOListIndex)
    {
        DeliverCorrectRecipeClientRpc(currentRecipeSOListIndex);
    }

    [ClientRpc]
    private void DeliverCorrectRecipeClientRpc(int currentRecipeSOListIndex)
    {
        numRecipesDelivered += 1;

        waitingRecipeSOList.RemoveAt(currentRecipeSOListIndex);

        OnUpdateWaitingRecipesList?.Invoke(this, EventArgs.Empty);
        OnDeliverRecipeSuccess?.Invoke(this, EventArgs.Empty);
    }

    public int GetNumRecipesDelivered()
    {
        return numRecipesDelivered;
    }
}
