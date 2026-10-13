using Fusion;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class PlayerHealth : NetworkBehaviour, IDamageable
{
    public int maxHealth = 100;
    [Networked] public int currentHealth { get; set; }
    [Networked] public NetworkBool isDead { get; set; }

    public static event Action<int> OnHealthChange;
    public static event Action<PlayerRef> OnPlayerDied;
    private ChangeDetector changeDetector;

    public override void Spawned()
    {
        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        if(HasStateAuthority)
        {
            currentHealth = maxHealth;
            isDead = false;
        }
        if (HasInputAuthority)
        {
            OnHealthChange?.Invoke(currentHealth);
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if(HasInputAuthority)
        {
            OnHealthChange?.Invoke(0);
        }
    }
/*
    private void Update()
    {
        if (HasInputAuthority && Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(9999);
        }
    }
*/
    public override void Render()
    {
        if(!HasInputAuthority)
        {
            return;
        }
        foreach (string change in changeDetector.DetectChanges(this))
        {
            if (change == nameof(currentHealth))
            {
                OnHealthChange?.Invoke(currentHealth);
            }
        }
    }

    public void TakeDamage(int damageInt)
    {
        if(!HasStateAuthority)
        {
            return;
        }
        if(isDead)
        {
            return;
        }
        currentHealth -= damageInt;
        currentHealth = Mathf.Max(currentHealth, 0);
        if (currentHealth <= 0)
        {
            isDead = true;
            OnPlayerDied?.Invoke(Object.InputAuthority);
            Runner.Despawn(Object);
        }
    }

}