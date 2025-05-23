using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField] private BaseCounter parent;
    [SerializeField] private GameObject[] selectedVisualArray;

    private void Start()
    {
        if (Player.LocalInstance != null)
        {
            Player.LocalInstance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
        }
        else
        {
            Player.OnAnyPlayerSpawned += Player_OnAnyPlayerSpawned;
        }
    }

    private void Player_OnAnyPlayerSpawned(object sender, System.EventArgs e)
    {
        if (Player.LocalInstance != null)
        {
            Player.LocalInstance.OnSelectedCounterChanged -= Player_OnSelectedCounterChanged;
            Player.LocalInstance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
            // when multiple players spawned, Player.LocalInstance.OnSelectedCounterChangde event that spawned before others
            // will be register multiple times. 
            // -> unregister then register will make this LocalInstance.OnSelectedCounterChanged only be registered once
        }
    }

    private void Player_OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e)
    {
        if (e.selectedCounter == parent)
        {
            foreach (var item in selectedVisualArray)
            {
                item.gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (var item in selectedVisualArray)
            {
                item.gameObject.SetActive(false);
            }
        }
    }
}
