using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterColorSelectSingleUI : MonoBehaviour
{
    [SerializeField] private int colorID;
    [SerializeField] private Image image;
    [SerializeField] private GameObject selectedGameObject;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            GameMultiplayerManager.Instance.ChangePlayerColor(colorID);
        });
    }

    private void Start()
    {
        GameMultiplayerManager.Instance.OnPlayerNetworkListChanged += GameMultiplayerManager_OnPlayerNetworkListChanged;
        image.color = GameMultiplayerManager.Instance.GetPlayerColor(colorID);
        SelectedVisualUpdate();
    }

    private void OnDestroy()
    {
        GameMultiplayerManager.Instance.OnPlayerNetworkListChanged -= GameMultiplayerManager_OnPlayerNetworkListChanged;
    }

    private void GameMultiplayerManager_OnPlayerNetworkListChanged(object sender, System.EventArgs e)
    {
        SelectedVisualUpdate();
    }

    private void SelectedVisualUpdate()
    {
        if(GameMultiplayerManager.Instance.GetLocalPlayerData().colorID == colorID)
        {
            selectedGameObject.SetActive(true);
        }
        else
        {
            selectedGameObject.SetActive(false);
        }
    }
}
