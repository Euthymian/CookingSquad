using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // This is time-base system

    public static GameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;

    private enum State
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    private State state;
    private float countdownToStartTimer = 1f;
    private float gamePlayingTimeMax = 300f;
    private float gamePlayingTimer = 300f;
    private bool isPausing = false;
    public bool IsPausing { get { return isPausing; } }

    private void Awake()
    {
        Instance = this;
        state = State.CountdownToStart;
    }

    private void Start()
    {
        GameInput.Instance.OnInteractPause += GameInput_OnInteractPause;
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if (state == State.WaitingToStart)
        {
            state = State.CountdownToStart;
            OnStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void GameInput_OnInteractPause(object sender, EventArgs e)
    {
        TogglePauseGame();
    }

    private void Update()
    {
        switch(state)
        {
            case State.WaitingToStart:
                break;
            case State.CountdownToStart:
                countdownToStartTimer -= Time.deltaTime;
                if (countdownToStartTimer <= 0f)
                {
                    state = State.GamePlaying;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GamePlaying:
                gamePlayingTimer -= Time.deltaTime;
                if (gamePlayingTimer <= 0f)
                {
                    gamePlayingTimer = gamePlayingTimeMax;
                    state = State.GameOver;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.GameOver:
                break;
        }
    }

    public int GetCountdownToStartTimer()
    {
        return (int)countdownToStartTimer;
    }

    public bool IsGamePlaying()
    {
        return state == State.GamePlaying;
    }

    public bool IsWaiting()
    {
        return state == State.WaitingToStart;
    }

    public bool IsCountdownToStart()
    {
        return state == State.CountdownToStart;
    }

    public bool IsGameOver()
    {
        return state == State.GameOver;
    }

    public float GetGamePlayingTimerNormalized()
    {
        return gamePlayingTimer/gamePlayingTimeMax;
    }

    public void TogglePauseGame()
    {
        if (state == State.GameOver) return;

        isPausing = !isPausing;

        if (isPausing) Time.timeScale = 0f;
        else Time.timeScale = 1f;
    }
}
