using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class ConnectingUI : MonoBehaviour
{
    private void Start()
    {
        GameMultiplayerManager.Instance.OnTryToJoinGame += GameMultiplayerManager_OnTryToJoinGame;
        GameMultiplayerManager.Instance.OnFailedToJoinGame += GameMultiplayerManager_OnFailedToJoinGame;
        Hide();
    }

    private void OnDestroy()
    {
        GameMultiplayerManager.Instance.OnTryToJoinGame -= GameMultiplayerManager_OnTryToJoinGame;
        GameMultiplayerManager.Instance.OnFailedToJoinGame -= GameMultiplayerManager_OnFailedToJoinGame;
    }

    private void GameMultiplayerManager_OnFailedToJoinGame(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void GameMultiplayerManager_OnTryToJoinGame(object sender, System.EventArgs e)
    {
        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
