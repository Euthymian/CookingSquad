using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class HostDisconnectedUI : MonoBehaviour
{
    [SerializeField] private Button playAgainButton;

    private void Awake()
    {
        playAgainButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown(); // Disconnect from server
            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }

    private void Start()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        Hide();
    }

    /*
     game started, client late joined, go to menu, rejoin (game still run) -> error becuase HostDisconnectedUI was destroyed in scenechanged but still try to access
    -> onDestroy, unregister the callback
            player line 63 also had this registration ---> consider unregister
     */

    private void OnDestroy()
    {
        print(NetworkManager.Singleton.IsServer);
        NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback;
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientID)
    {
        if(clientID == NetworkManager.ServerClientId)
        {
            Show();
        }
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
