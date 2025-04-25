using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress
{
    public event EventHandler<float> OnProgessUpdate;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }

    public enum State // enum is serializable -> use networkvariable
    {
        Idle, 
        Frying,
        Fried,
        Burned
    }

    private NetworkVariable<State> state = new NetworkVariable<State>(State.Idle);
    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;
    private NetworkVariable<float> fryingTime = new NetworkVariable<float>(0);
    // Reason of using networkvariable is that networkvarible only exists on server -> only 1 process through every client
    private NetworkVariable<float> burningTime = new NetworkVariable<float>(0);
    private FryingRecipeSO fryingRecipeSO;
    private BurningRecipeSO burningRecipeSO;

    public override void OnNetworkSpawn()
    {
        fryingTime.OnValueChanged += FryingTime_OnValueChanged;
        burningTime.OnValueChanged += BurningTime_OnValueChanged;
        state.OnValueChanged += State_OnValueChanged;
    }

    private void FryingTime_OnValueChanged(float prevValue, float newValue)
    {
        // this line is because the event is fired on OnNetworkSpawn()
        // at first, fryingRecipeSO = null -> if use fryingRecipeSO.fryingTimeMax as denominator, NullExceptionError
        // so if fryingRecipeSO = null, fryingTimeMax = 1 as default
        float fryingTimeMax = fryingRecipeSO != null ? fryingRecipeSO.fryingTimeMax : 1f;

        OnProgessUpdate?.Invoke(this, fryingTime.Value / fryingTimeMax);
    }

    private void BurningTime_OnValueChanged(float prevValue, float newValue)
    {
        float burningTimeMax = burningRecipeSO != null ? burningRecipeSO.burningTimeMax : 1f;

        OnProgessUpdate?.Invoke(this, burningTime.Value / burningTimeMax);
    }

    private void State_OnValueChanged(State prevValue, State newValue)
    {
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs()
        {
            state = state.Value
        });

        if(state.Value == State.Burned || state.Value == State.Idle)
        {
            OnProgessUpdate?.Invoke(this, 1);
        }
    }

    private void Update()
    {
        if (!IsServer) return;

        if (HasKitchenObject())
        {
            switch (state.Value)
            {
                case State.Idle:
                    break;
                case State.Frying:
                    fryingTime.Value += Time.deltaTime;

                    if (fryingTime.Value > fryingRecipeSO.fryingTimeMax)
                    {
                        KitchenObject.DestroyKitchentObject(GetKitchenObject());
                        KitchenObject.SpawnKitchenObject(fryingRecipeSO.output, this);
                        burningTime.Value = 0f;
                        state.Value = State.Fried;

                        SetBurningRecipeSOClientRpc(
                            GameMultiplayerManager.Instance.GetKitchenObjectSOIndex(GetKitchenObject().GetKitchenObjectSO())    
                        );
                    }
                    break;
                case State.Fried:
                    burningTime.Value += Time.deltaTime;
                    
                    if (burningTime.Value > burningRecipeSO.burningTimeMax)
                    {
                        KitchenObject.DestroyKitchentObject(GetKitchenObject());
                        KitchenObject.SpawnKitchenObject(burningRecipeSO.output, this);
                        burningTime.Value = 0f;
                        state.Value = State.Burned;
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
            KitchenObject kitchenObject = player.GetKitchenObject();
            kitchenObject.SetKitchenObjectParent(this);
            // This will tell every client what is fryingRecipeSO now
            InteractSyncServerRpc(
                GameMultiplayerManager.Instance.GetKitchenObjectSOIndex(kitchenObject.GetKitchenObjectSO())
            );
            /*
                In single player base game, we grapped fryingRecipeSO by 
                         fryingRecipeSO = GetKitchenObject().GetKitchenObjectSO()
                but it is recommened to use player.GetKitchenObject() instead of GetKitchenObject() (THE SAME AS ABOVE) is becuase
                from above, we set kitchenObject of player to this counter, but sometimes becuase of delay, the server loaded that action but the client didnt
                -> GetKitchenObject() would be null -> NullException
            */
        }
        else if (HasKitchenObject() && !player.HasKitchenObject())
        {
            GetKitchenObject().SetKitchenObjectParent(player);

            SetIdleStateAfterPickupServerRpc(); // <- Cant write NetworkVariable directly on client
        }
        else if (HasKitchenObject() && player.HasKitchenObject())
        {
            if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                {
                    KitchenObject.DestroyKitchentObject(GetKitchenObject());

                    SetIdleStateAfterPickupServerRpc();
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetIdleStateAfterPickupServerRpc()
    {
        state.Value = State.Idle;
    }
 
    [ServerRpc(RequireOwnership = false)]
    private void InteractSyncServerRpc(int kitchenObjectSOIndex)
    {
        fryingTime.Value = 0;
        state.Value = State.Frying;
        SetFryingRecipeSOClientRpc(kitchenObjectSOIndex);
    }

    [ClientRpc]
    private void SetFryingRecipeSOClientRpc(int kitchenObjectSOIndex)
    {
        //fryingTime.Value = 0;         |-> These will be run on ServerRpc becuase only server can write NetworkVariable
        //state.Value = State.Frying;   |
        KitchenObjectSO kitchenObjectSO = GameMultiplayerManager.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        fryingRecipeSO = GetMatchedFryingRecipe(kitchenObjectSO);
    }

    [ClientRpc]
    private void SetBurningRecipeSOClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObjectSO = GameMultiplayerManager.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        burningRecipeSO = GetMatchedBurningRecipe(kitchenObjectSO);
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

    public bool IsFried()
    {
        return state.Value == State.Fried;
    }

}
