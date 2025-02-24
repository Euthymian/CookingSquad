using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress
{
    public event EventHandler<float> OnProgessUpdate;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }

    public enum State
    {
        Idle, 
        Frying,
        Fried,
        Burned
    }

    private State state;
    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;
    private float fryingTime;
    private float burningTime;
    private FryingRecipeSO fryingRecipeSO;
    private BurningRecipeSO burningRecipeSO;

    private void Start()
    {
        state = State.Idle;
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs()
        {
            state = state
        });
    }

    private void Update()
    {
        if (HasKitchenObject())
        {
            switch (state)
            {
                case State.Idle:
                    break;
                case State.Frying:
                    fryingTime += Time.deltaTime;
                    OnProgessUpdate?.Invoke(this, fryingTime / fryingRecipeSO.fryingTimeMax);
                    if (fryingTime > fryingRecipeSO.fryingTimeMax)
                    {
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);
                        burningRecipeSO = GetMatchedBurningRecipe(GetKitchenObject().GetKitchenObjectSO());
                        burningTime = 0f;
                        state = State.Fried;
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs()
                        {
                            state = state
                        });
                        OnProgessUpdate?.Invoke(this, 0);
                    }
                    break;
                case State.Fried:
                    burningTime += Time.deltaTime;
                    OnProgessUpdate?.Invoke(this, burningTime / burningRecipeSO.burningTimeMax);
                    if (burningTime > burningRecipeSO.burningTimeMax)
                    {
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);
                        burningTime = 0f;
                        state = State.Burned;
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs()
                        {
                            state = state
                        });
                        OnProgessUpdate?.Invoke(this, 1);
                    }
                    break;
                case State.Burned:
                    break;
            }
        }
    }

    public override void Interact(Player player)
    {
        if (!HasKitchenObject() && player.HasKitchenObject()
            && GetMatchedFryingRecipe(player.GetKitchenObject().GetKitchenObjectSO()))
        {
            player.GetKitchenObject().SetKitchenObjectParent(this);
            state = State.Frying;
            OnStateChanged?.Invoke(this, new OnStateChangedEventArgs()
            {
                state = state
            });
            fryingTime = 0;
            fryingRecipeSO = GetMatchedFryingRecipe(GetKitchenObject().GetKitchenObjectSO());
            OnProgessUpdate?.Invoke(this, fryingTime/fryingRecipeSO.fryingTimeMax);
        }
        else if (HasKitchenObject() && !player.HasKitchenObject())
        {
            GetKitchenObject().SetKitchenObjectParent(player);
            state = State.Idle;
            OnStateChanged?.Invoke(this, new OnStateChangedEventArgs()
            {
                state = state
            });
            OnProgessUpdate?.Invoke(this, 0);
        }
        else if (HasKitchenObject() && player.HasKitchenObject())
        {
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                {
                    GetKitchenObject().DestroySelf();

                    state = State.Idle;
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs()
                    {
                        state = state
                    });
                    OnProgessUpdate?.Invoke(this, 0);
                }
            }
        }
    }

    private FryingRecipeSO GetMatchedFryingRecipe(KitchenObjectSO kitchenObjectSO)
    {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray)
        {
            if (fryingRecipeSO.input == kitchenObjectSO)
            {
                return fryingRecipeSO;
            }
        }
        return null;
    }

    private BurningRecipeSO GetMatchedBurningRecipe(KitchenObjectSO kitchenObjectSO)
    {
        foreach (BurningRecipeSO burningRecipeSO in burningRecipeSOArray)
        {
            if (burningRecipeSO.input == kitchenObjectSO)
            {
                return burningRecipeSO;
            }
        }
        return null;
    }
}
