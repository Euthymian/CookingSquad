using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausedUI : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button mainMenuButton;
    private bool isPausing = false;

    private void Awake()
    {
        resumeButton.onClick.AddListener(() =>
        {
            isPausing = false;
            GameManager.Instance.TogglePauseGame();
            Hide();
        });

        mainMenuButton.onClick.AddListener(() => 
        { 
            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }

    private void Start()
    {
        GameInput.Instance.OnInteractPause += GameInput_OnInteractPause;
        gameObject.SetActive(false);
    }

    private void GameInput_OnInteractPause(object sender, System.EventArgs e)
    {
        TogglePauseMenu();
    }

    private void TogglePauseMenu()
    {
        if (GameManager.Instance.IsGameOver()) return;

        isPausing = !isPausing;

        if (isPausing) Show();
        else Hide();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}
