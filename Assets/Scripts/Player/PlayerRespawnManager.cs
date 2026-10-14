using System.Collections;
using Fusion;
using UnityEngine;

public class PlayerRespawnManager : MonoBehaviour //again MonoBehavior because we are going to get the runner manually
{
    [SerializeField] private NetworkPrefabRef playerPrefab;
    [SerializeField] private float respawnDelay = 3f;
    private NetworkRunner runner;

    private void OnEnable()
    {
        PlayerHealth.OnPlayerDied += HandlePlayerDied;
    }

    private void OnDisable()
    {
        PlayerHealth.OnPlayerDied -= HandlePlayerDied;
    }

    private NetworkRunner GetRunner()
    {
        if (runner == null)
        {
            runner = FindFirstObjectByType<NetworkRunner>();
        }
        return runner;
    }

    private void HandlePlayerDied(PlayerRef player)
    {
        NetworkRunner runner = GetRunner();
        if(runner == null || !runner.IsServer)
        {
            return;
        }
        StartCoroutine(RespawnRoutine(runner, player));
    }

    private IEnumerator RespawnRoutine(NetworkRunner runner, PlayerRef player)
    {
        //deathCamera.Play(playerCamera.position, playerCamera.rotation);
        yield return new WaitForSeconds(respawnDelay);
        if(IsPlayerConnected(runner, player) == false)
        {
            yield break;
        }
        Respawn(runner, player);
    }

    private bool IsPlayerConnected(NetworkRunner runner, PlayerRef player)
    {
        foreach (PlayerRef activePlayer in runner.ActivePlayers)
        {
            if (activePlayer == player)
            {
                return true;
            }
        }
        return false;
    }

    private void Respawn(NetworkRunner runner, PlayerRef player)
    {
        Transform point = PlayerSpawnManager.Instance.GetRandomSpawnPoint();
        NetworkObject playerObj = runner.Spawn(playerPrefab, point.position, point.rotation, player);
        runner.SetPlayerObject(player, playerObj);
        //deathCamera.Stop();
    }

}
