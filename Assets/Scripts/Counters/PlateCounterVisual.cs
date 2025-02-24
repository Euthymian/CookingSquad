using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateCounterVisual : MonoBehaviour
{
    [SerializeField] private PlateCounter plateCounter;
    [SerializeField] private Transform holdObjectPoint;
    [SerializeField] private Transform platVisualPrefab;

    private List<GameObject> plateVisualList;

    private void Awake()
    {
        plateVisualList = new List<GameObject>();
    }

    private void Start()
    {
        plateCounter.OnPlateSpawned += PlateCounter_OnPlateSpawned;
        plateCounter.OnPlateRemoved += PlateCounter_OnPlateRemoved;
    }

    private void PlateCounter_OnPlateRemoved(object sender, System.EventArgs e)
    {
        GameObject lastPlateVisual = plateVisualList[plateVisualList.Count - 1];
        plateVisualList.Remove(lastPlateVisual);
        Destroy(lastPlateVisual);
    }

    private void PlateCounter_OnPlateSpawned(object sender, System.EventArgs e)
    {
        Transform plateVisualTransform = Instantiate(platVisualPrefab, holdObjectPoint);

        float plateOffsetY = 0.1f;
        plateVisualTransform.localPosition = new Vector3(0, plateOffsetY * plateVisualList.Count, 0);

        plateVisualList.Add(plateVisualTransform.gameObject);
    }
}
