using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingUnpausedUI : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.OnMultiPlayerGamePaused += GameManager_OnMultiPlayerGamePaused;
        GameManager.Instance.OnMultiPlayerGameUnpaused += GameManager_OnMultiPlayerGameUnpaused;
        Hide();
    }

    private void GameManager_OnMultiPlayerGameUnpaused(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void GameManager_OnMultiPlayerGamePaused(object sender, System.EventArgs e)
    {
        print("waitpause");
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
