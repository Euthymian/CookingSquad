using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
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
        spawnRecipeTimer -= Time.deltaTime;
        if (spawnRecipeTimer <= 0) {
            spawnRecipeTimer = spawnRecipeTimerMax;
            
            if(waitingRecipeSOList.Count < maxRecipes)
            {
                RecipeSO newWaitingRecipe = availableRecipeSO.recipeSOList[UnityEngine.Random.Range(0, availableRecipeSO.recipeSOList.Count)];
                waitingRecipeSOList.Add(newWaitingRecipe);
                
                OnUpdateWaitingRecipesList?.Invoke(this, EventArgs.Empty);
            }
        }
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
                    }
                }

                if (plateContentMatchesRecipe)
                {
                    numRecipesDelivered += 1;

                    waitingRecipeSOList.RemoveAt(i);

                    OnUpdateWaitingRecipesList?.Invoke(this, EventArgs.Empty);
                    OnDeliverRecipeSuccess?.Invoke(this, EventArgs.Empty);

                    return;
                }
            }
        }
        
        OnDeliverRecipeFail?.Invoke(this, EventArgs.Empty);
    }

    public int GetNumRecipesDelivered()
    {
        return numRecipesDelivered;
    }
}
