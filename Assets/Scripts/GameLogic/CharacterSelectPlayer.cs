using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour
{
    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject readyTextGameObject;
    [SerializeField] private PlayerVisual playerVisual;
    [SerializeField] private Button kickButton;
    [SerializeField] private TextMeshPro playerNameText;

    private void Awake()
    {
        kickButton.onClick.AddListener(() =>
        {
            PlayerData playerData = GameMultiplayerManager.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            LobbyManager.Instance.KickPlayer(playerData.playerID.ToString());
            GameMultiplayerManager.Instance.KickPlayer(playerData.clientID);
        });
    }

    private void Start()
    {
        GameMultiplayerManager.Instance.OnPlayerNetworkListChanged += GameMultiplayerManager_OnPlayerNetworkListChanged;
        CharacterSelectReady.Instance.OnPlayerReadyChanged += CharacterSelectReady_OnPlayerReadyChanged;

        kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);
        //if (NetworkManager.ServerClientId == GameMultiplayerManager.Instance.GetPlayerDataFromPlayerIndex(playerIndex).clientID)
        //{
        //    kickButton.gameObject.SetActive(false);
        //}
        UpdatePlayer();
    }

    private void OnDestroy()
    {
        GameMultiplayerManager.Instance.OnPlayerNetworkListChanged -= GameMultiplayerManager_OnPlayerNetworkListChanged;
        CharacterSelectReady.Instance.OnPlayerReadyChanged -= CharacterSelectReady_OnPlayerReadyChanged;
    }

    private void CharacterSelectReady_OnPlayerReadyChanged(object sender, System.EventArgs e)
    {
        UpdatePlayer();
    }

    private void GameMultiplayerManager_OnPlayerNetworkListChanged(object sender, System.EventArgs e)
    {
        UpdatePlayer();
    }

    private void UpdatePlayer()
    {
        if (GameMultiplayerManager.Instance.IsPlayerIndexConnected(playerIndex))
        {
            Show();
            PlayerData playerData = GameMultiplayerManager.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            playerVisual.SetColor(GameMultiplayerManager.Instance.GetPlayerColor(playerData.colorID));
            playerNameText.text = playerData.playerName.ToString();
            readyTextGameObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientID));
        }
        else
        {
            Hide();
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
