using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    [SerializeField] private BaseCounter parent;
    [SerializeField] private GameObject[] selectedVisualArray;

    private void Start()
    {
        Player.Instance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;  
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
