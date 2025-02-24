using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour
{
    [Serializable]
    public struct KitchenObjectSO_GameObject
    {
        public KitchenObjectSO kitchenObjectSO;
        public GameObject gameObject;
    }

    [SerializeField] private PlateKitchenObject kitchenObject;
    [SerializeField] private KitchenObjectSO_GameObject[] kitchenObjectSO_GameObjectArray;

    private void Start()
    {
        kitchenObject.OnIngredientAdded += KitchenObject_OnIngredientAdded;
        foreach (KitchenObjectSO_GameObject each in kitchenObjectSO_GameObjectArray)
        {
            each.gameObject.SetActive(false);
        }
    }

    private void KitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
    {
        foreach (KitchenObjectSO_GameObject each in kitchenObjectSO_GameObjectArray)
        {
            if (each.kitchenObjectSO == e.kitchenObjectSO)
            {
                each.gameObject.SetActive(true);
                break;
            }
        }
    }
}
