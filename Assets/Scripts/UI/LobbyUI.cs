using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button quickJoinButton;
    [SerializeField] private TMP_InputField codeInputField;
    [SerializeField] private Button codeJoinButton;
    [SerializeField] private LobbyCreateUI lobbyCreateUI;
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private Transform lobbyListContainer;
    [SerializeField] private Transform lobbyButtonTemplate;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.LeaveLobby();
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenuScene);
        });

        createLobbyButton.onClick.AddListener(() =>
        {
            lobbyCreateUI.Show();
        });

        quickJoinButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.QuickJoin();
        });

        codeJoinButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.CodeJoin(codeInputField.text);
        });

        lobbyButtonTemplate.gameObject.SetActive(false );
    }

    private void Start()
    { 
        playerNameInputField.text = GameMultiplayerManager.Instance.GetPlayerName();
        playerNameInputField.onValueChanged.AddListener((string newValue) =>
        {
            GameMultiplayerManager.Instance.SetPlayerName(newValue);
        });

        LobbyManager.Instance.OnLobbyListChanged += LobbyManager_OnLobbyListChanged;
        UpdateLobbyList(new List<Lobby>());
    }

    private void OnDestroy()
    {
        LobbyManager.Instance.OnLobbyListChanged -= LobbyManager_OnLobbyListChanged;
    }

    private void LobbyManager_OnLobbyListChanged(object sender, LobbyManager.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }

    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        foreach(Transform child in lobbyListContainer)
        {
            if(child == lobbyButtonTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach(Lobby lobby in lobbyList)
        {
            Transform lobbyTransform = Instantiate(lobbyButtonTemplate, lobbyListContainer);
            lobbyTransform.gameObject.SetActive(true);
            lobbyTransform.GetComponent<LobbyListSingleUI>().SetLobby(lobby);
        }
    }
}
