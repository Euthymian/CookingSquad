using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameStartCountdownUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;
    private Animator anim;
    private int previousCountdownValue = -1;
    private const string COUNTDOWN_ANIMATION_TRIGGER = "NumberPopup";

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        Hide();
    }

    private void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsCountdownToStart())
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Update()
    {
        int currentCountdownValue = GameManager.Instance.GetCountdownToStartTimer() + 1;
        if(currentCountdownValue != previousCountdownValue)
        {
            print(currentCountdownValue);
            countdownText.text = currentCountdownValue.ToString();
            anim.SetTrigger(COUNTDOWN_ANIMATION_TRIGGER);
            //anim.ResetTrigger(COUNTDOWN_ANIMATION_TRIGGER);
            SoundManager.Instance.PlayCountdownNumberPopupSound();           
            previousCountdownValue = currentCountdownValue;
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
