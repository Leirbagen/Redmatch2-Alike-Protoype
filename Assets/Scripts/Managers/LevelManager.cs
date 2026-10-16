using Fusion;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : NetworkBehaviour
{
    public static LevelManager Instance { get; private set; }

    public float timeLimitSeconds = 120f;

    [Networked] public TickTimer MatchTimer { get; set; }

    [Networked, Capacity(8)]
    public NetworkDictionary<PlayerRef, int> PlayerKills => default;
    [Networked, OnChangedRender(nameof(OnMatchOverChanged))]
    public NetworkBool IsMatchOver { get; set; }

    [Networked] public PlayerRef WinnerPlayer { get; set; }
    [Networked] public int WinnerKillsAmount { get; set; }

    public static event Action<float> OnTimeUpdated;
    public static event Action<string, int> OnMatchEnded;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public override void FixedUpdateNetwork()
    {
        if (IsMatchOver) return;
        if (HasStateAuthority && !MatchTimer.IsRunning)
        {
            MatchTimer = TickTimer.CreateFromSeconds(Runner, timeLimitSeconds);
        }
        if (MatchTimer.IsRunning)
        {
            float? remaining = MatchTimer.RemainingTime(Runner);
            if (remaining.HasValue)
            {
                OnTimeUpdated?.Invoke(remaining.Value);
            }
        }
        if (HasStateAuthority && MatchTimer.IsRunning && MatchTimer.Expired(Runner))
        {
            CalculateWinner();
        }
    }

    public void AddKill(PlayerRef killer)
    {
        if (!HasStateAuthority || IsMatchOver || !killer.IsValid) return;

        int currentKills = PlayerKills.ContainsKey(killer) ? PlayerKills.Get(killer) : 0;
        PlayerKills.Set(killer, currentKills + 1);
    }

    private void CalculateWinner()
    {
        PlayerRef winner = PlayerRef.None;
        int maxKills = 0; 

        foreach (var kvp in PlayerKills)
        {
            if (kvp.Value > maxKills)
            {
                maxKills = kvp.Value;
                winner = kvp.Key;
            }
        }

        WinnerPlayer = winner;
        WinnerKillsAmount = maxKills;
        IsMatchOver = true;
    }

    private void OnMatchOverChanged()
    {
        string winnerName = WinnerPlayer.IsValid ? $"Player {WinnerPlayer.PlayerId}" : "Empate / Nadie";
        OnMatchEnded?.Invoke(winnerName, WinnerKillsAmount);
    }
}