using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField lobbyNameInputField;
    [SerializeField] private Button createPublicButton;
    [SerializeField] private Button createPrivateButton;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        createPublicButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.CreateLobby(lobbyNameInputField.text, false);
        });

        createPrivateButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.CreateLobby(lobbyNameInputField.text, true);
        });

        closeButton.onClick.AddListener(() =>
        {
            Hide();
        });
    }

    private void Start()
    {
        Hide();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        createPublicButton.Select();
    }
}
