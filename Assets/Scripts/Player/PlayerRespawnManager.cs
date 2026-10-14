using System.Collections;
using Fusion;
using UnityEngine;

public class PlayerRespawnManager : SimulationBehaviour
{
    [SerializeField] private NetworkPrefabRef playerPrefab;
    [SerializeField] private float respawnDelay = 3f;

    private void OnEnable()
    {
        PlayerHealth.OnPlayerDied += HandlePlayerDied;
    }

    private void OnDisable()
    {
        PlayerHealth.OnPlayerDied -= HandlePlayerDied;
    }

    private void HandlePlayerDied(PlayerRef player)
    {
        Debug.Log($"[Respawn] murió {player}, Runner={(Runner != null)}, IsServer={(Runner != null && Runner.IsServer)}");
        if(Runner == null || !Runner.IsServer)
        {
            return;
        }
        StartCoroutine(RespawnRoutine(player));
    }

    private IEnumerator RespawnRoutine(PlayerRef player)
    {
        //deathCamera.Play(playerCamera.position, playerCamera.rotation);
        yield return new WaitForSeconds(respawnDelay);
        if(IsPlayerConnected(player) == false)
        {
            yield break;
        }
        Respawn(player);
    }

    private bool IsPlayerConnected(PlayerRef player)
    {
        foreach (PlayerRef activePlayer in Runner.ActivePlayers)
        {
            if (activePlayer == player)
            {
                return true;
            }
        }
        return false;
    }

    private void Respawn(PlayerRef player)
    {
        Transform point = PlayerSpawnManager.Instance.GetRandomSpawnPoint();
        NetworkObject playerObj = Runner.Spawn(playerPrefab, point.position, point.rotation, player);
        Runner.SetPlayerObject(player, playerObj);
        //deathCamera.Stop();
    }

}
