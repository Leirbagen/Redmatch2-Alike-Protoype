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
    [SerializeField] private GameObject playerFractured;
    private ChangeDetector changeDetector;
    public AudioSource healthAudioSource;
    public AudioClip[] painSounds; 
    private int previousHealth;

    public override void Spawned()
    {
        changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        if(HasStateAuthority)
        {
            currentHealth = maxHealth;
            isDead = false;
        }
        previousHealth = currentHealth;
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
            SpawnFracturedPlayer();
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
        foreach (string change in changeDetector.DetectChanges(this))
        {
            if (change == nameof(currentHealth))
            {
                if (currentHealth < previousHealth && !isDead)
                {
                    if (healthAudioSource != null && painSounds.Length > 0)
                    {
                        AudioClip randomPain = painSounds[UnityEngine.Random.Range(0, painSounds.Length)];
                        healthAudioSource.PlayOneShot(randomPain);
                    }
                }
            }
            if (HasInputAuthority)
            {
                OnHealthChange?.Invoke(currentHealth);
            }
            previousHealth = currentHealth;
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

    private void SpawnFracturedPlayer()
    {
        if (playerFractured != null)
        {
            GameObject corpseClone = Instantiate(playerFractured, transform.position, transform.rotation);
            Destroy(corpseClone, 5f);
        }
    }

}