using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    // This is time-base system

    public static GameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;

    public event EventHandler OnIsLocalPlayerReadyChanged;

    public event EventHandler OnLocalGamePaused;
    public event EventHandler OnLocalGameUnpaused;

    public event EventHandler OnMultiPlayerGamePaused;
    public event EventHandler OnMultiPlayerGameUnpaused;

    private enum State
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver,
    }

    private NetworkVariable<State> state = new NetworkVariable<State>(State.WaitingToStart);
    private bool isLocalPlayerReady;
    private NetworkVariable<float> countdownToStartTimer = new NetworkVariable<float>(3f);
    private float gamePlayingTimeMax = 300f;
    private NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>();
    private Dictionary<ulong, bool> playersReadyDict;
    private bool isLocalPausing = false;
    private Dictionary<ulong, bool> playersPauseDict;
    private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false);
    private bool autoTestGamePausedState;
    public bool IsLocalPausing { get { return isLocalPausing; } }

    private void Awake()
    {
        Instance = this;
        playersReadyDict = new Dictionary<ulong, bool>();
        playersPauseDict = new Dictionary<ulong, bool>();
    }

    public override void OnNetworkSpawn()
    {
        gamePlayingTimer.Value = gamePlayingTimeMax;
        state.OnValueChanged += State_OnValueChanged;
        isGamePaused.OnValueChanged += IsGamePaused_OnValueChanged;

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientID)
    {
        autoTestGamePausedState = true;
        /*
         Scenario: When player that paused quit game, the WaitingToUnpuased stayed on other players screen
         Cant directly call CheckGamePauseState(); here becuase after fire the DisconnectCallback, the disconnected clientID is still in the clientIDList 
        of networkManager -> The waiting to unpaused UI will stay.
        -> Solution: Wait after 1 frame so the new clientIDList is now uptodate then run the CheckGamePauseState();
         */
    }

    private void LateUpdate()
    {
        if (autoTestGamePausedState)
        {
            autoTestGamePausedState = false;
            CheckGamePauseState();
        }
    }

    private void State_OnValueChanged(State prevValue, State newValue)
    {
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void IsGamePaused_OnValueChanged(bool prevValue, bool newValue)
    {
        if(isGamePaused.Value)
        {
            Time.timeScale = 0; 
            OnMultiPlayerGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1;
            OnMultiPlayerGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }

    private void Start()
    {
        GameInput.Instance.OnInteractPause += GameInput_OnInteractPause;
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if (state.Value == State.WaitingToStart)
        {
            isLocalPlayerReady = true;
            OnIsLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);
            SetPlayerReadyServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playersReadyDict[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;
        foreach (ulong clientID in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playersReadyDict.ContainsKey(clientID) || !playersReadyDict[clientID])
            {
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady)
        {
            state.Value = State.CountdownToStart;
        }
    }

    private void GameInput_OnInteractPause(object sender, EventArgs e)
    {
        TogglePauseGame();
    }

    private void Update()
    {
        if (!IsServer) return;

        switch (state.Value)
        {
            case State.WaitingToStart:
                break;
            case State.CountdownToStart:
                countdownToStartTimer.Value -= Time.deltaTime;
                if (countdownToStartTimer.Value <= 0f)
                {
                    state.Value = State.GamePlaying;
                }
                break;
            case State.GamePlaying:
                gamePlayingTimer.Value -= Time.deltaTime;
                if (gamePlayingTimer.Value <= 0f)
                {
                    gamePlayingTimer.Value = gamePlayingTimeMax;
                    state.Value = State.GameOver;
                }
                break;
            case State.GameOver:
                break;
        }
    }

    public bool IsLocalPlayerReady()
    {
        return isLocalPlayerReady;
    }

    public int GetCountdownToStartTimer()
    {
        return (int)countdownToStartTimer.Value;
    }

    public bool IsGamePlaying()
    {
        return state.Value == State.GamePlaying;
    }

    public bool IsWaiting()
    {
        return state.Value == State.WaitingToStart;
    }

    public bool IsCountdownToStart()
    {
        return state.Value == State.CountdownToStart;
    }

    public bool IsGameOver()
    {
        return state.Value == State.GameOver;
    }

    public float GetGamePlayingTimerNormalized()
    {
        return gamePlayingTimer.Value / gamePlayingTimeMax;
    }

    public void TogglePauseGame()
    {
        if (state.Value == State.GameOver) return;

        isLocalPausing = !isLocalPausing;

        if (isLocalPausing)
        {
            PauseGameServerRpc();
            OnLocalGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            UnPauseGameServerRpc();
            OnLocalGameUnpaused?.Invoke(this, EventArgs.Empty);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playersPauseDict[serverRpcParams.Receive.SenderClientId] = true;
        CheckGamePauseState();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UnPauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playersPauseDict[serverRpcParams.Receive.SenderClientId] = false;
        CheckGamePauseState();
    }

    private void CheckGamePauseState()
    {
        foreach (ulong clientID in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (playersPauseDict.ContainsKey(clientID) && playersPauseDict[clientID])
            {
                // A player paused game
                isGamePaused.Value = true;
                return;
            }
        }

        isGamePaused.Value = false;
        // No one paused
    }
}
