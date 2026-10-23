using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : NetworkBehaviour
{
    public static LevelManager Instance { get; private set; }

    public float timeLimitSeconds = 120f;
    public string mainMenuSceneName = "TitleScreen";

    [Networked] public TickTimer MatchTimer { get; set; }

    [Networked, Capacity(8)]
    public NetworkDictionary<PlayerRef, int> PlayerKills => default;

    [Networked, OnChangedRender(nameof(OnMatchOverChanged))]
    public NetworkBool IsMatchOver { get; set; }

    [Networked] public PlayerRef WinnerPlayer { get; set; }

    [Networked] public int WinnerKillsAmount { get; set; }
    [Networked] public NetworkBool IsKingHillMode { get; set; }

    public static event Action<float> OnTimeUpdated;
    public static event Action<string, int> OnMatchEnded;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public override void Spawned()
    {
        if (HasStateAuthority && BasicSpawner.Instance != null)
        {
            IsKingHillMode = BasicSpawner.Instance.CurrentGameType == CustomGameType.KingOfTheHill;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (IsMatchOver) return;

        if (HasStateAuthority && !MatchTimer.IsRunning)
        {
            MatchTimer = TickTimer.CreateFromSeconds(Runner, timeLimitSeconds);
        }

        if (HasStateAuthority && MatchTimer.IsRunning && MatchTimer.Expired(Runner))
        {
            CalculateWinner();
        }
    }

    public override void Render()
    {
        if (IsMatchOver) return;

        if (MatchTimer.IsRunning)
        {
            float? remaining = MatchTimer.RemainingTime(Runner);
            if (remaining.HasValue)
            {
                OnTimeUpdated?.Invoke(remaining.Value);
            }
        }
    }

    public void AddKill(PlayerRef killer)
    {
        if (!HasStateAuthority || IsMatchOver || !killer.IsValid) return;

        int currentKills = PlayerKills.ContainsKey(killer) ? PlayerKills.Get(killer) : 0;
        PlayerKills.Set(killer, currentKills + 1);
    }

    public void DeclareKingHillWinner(PlayerRef winner, float finalProgress)
    {
        if (!HasStateAuthority || IsMatchOver) return;

        WinnerPlayer = winner;
        WinnerKillsAmount = Mathf.FloorToInt(finalProgress);
        IsMatchOver = true;
    }

    private void CalculateWinner()
    {
        PlayerRef winner = PlayerRef.None;
        int finalScore = 0;

        if (IsKingHillMode)
        {
            float maxProgress = -1f;
            PlayerController[] allPlayers = FindObjectsOfType<PlayerController>();

            foreach (PlayerController player in allPlayers)
            {
                if (player.CaptureProgress > maxProgress)
                {
                    maxProgress = player.CaptureProgress;
                    winner = player.Object.InputAuthority;
                }
            }
            finalScore = Mathf.FloorToInt(maxProgress);
        }
        else
        {
            foreach (var kvp in PlayerKills)
            {
                if (kvp.Value > finalScore)
                {
                    finalScore = kvp.Value;
                    winner = kvp.Key;
                }
            }
        }

        WinnerPlayer = winner;
        WinnerKillsAmount = finalScore;
        IsMatchOver = true;
    }

    private void OnMatchOverChanged()
    {
        string winnerName = WinnerPlayer.IsValid ? $"Player {WinnerPlayer.PlayerId}" : "No one";
        OnMatchEnded?.Invoke(winnerName, WinnerKillsAmount);

        StartCoroutine(ReturnToMenuRoutine());
    }

    private IEnumerator ReturnToMenuRoutine()
    {
        yield return new WaitForSeconds(15f);

        if (Runner != null)
        {
            Runner.Shutdown();
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}