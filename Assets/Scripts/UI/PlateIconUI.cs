using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateIconUI : MonoBehaviour
{
    [SerializeField] PlateKitchenObject plateKitchenObject;
    [SerializeField] Transform iconTemplate;

    private void Awake()
    {
        iconTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
    }

    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        foreach (Transform each in transform)
        {
            if (each == iconTemplate) continue;
            Destroy(each.gameObject);
        }

        foreach (KitchenObjectSO kitchenObjectSO in plateKitchenObject.GetListKitchenObjectList())
        {
            Transform iconTemplateTransform = Instantiate(iconTemplate, transform);
            iconTemplateTransform.gameObject.SetActive(true);
            iconTemplateTransform.GetComponentInChildren<PlateIconSingleUI>().SetKitchenObjectSO(kitchenObjectSO);
        }
    }
}
