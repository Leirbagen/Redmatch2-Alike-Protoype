using Fusion;
using UnityEngine;
using System;

public class PlayerHealth : NetworkBehaviour, IDamageable
{
    public int maxHealth = 100;
    [Networked] public int currentHealth { get; set; }
    [Networked] public NetworkBool isDead { get; set; }

    public static event Action<int> OnHealthChange;
    public static event Action<PlayerRef> OnPlayerDied;
    [SerializeField] private Transform cameraTransform;
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
            PlayerDeathCamera.Instance?.Stop();
            OnHealthChange?.Invoke(currentHealth);
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if(HasInputAuthority)
        {
            OnHealthChange?.Invoke(0);
            if(hasState && cameraTransform != null) //hasState will check why the object has been despawned, if it was natural or because the runner went off
            {
                PlayerDeathCamera.Instance?.Play(cameraTransform.position, cameraTransform.rotation);
            }
        }
    }

    private void Update()
    {
        if (HasInputAuthority && Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(9999);
        }
    }

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

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_TakeDamage(int damage)
    {
        TakeDamage(damage);
    }

}