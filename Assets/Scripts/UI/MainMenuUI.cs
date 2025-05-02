using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button singleplayButton;
    [SerializeField] private Button multiplayButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        Time.timeScale = 1f;

        singleplayButton.onClick.AddListener(() =>
        {
            GameMultiplayerManager.isMultiplayer = false;
            Loader.Load(Loader.Scene.LobbyScene);
        });

        multiplayButton.onClick.AddListener(() =>
        {
            GameMultiplayerManager.isMultiplayer = true;
            Loader.Load(Loader.Scene.LobbyScene);
        });

        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        singleplayButton.Select();
    }
}
