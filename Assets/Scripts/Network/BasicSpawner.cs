using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private int sceneIndex = 2;
    [SerializeField] private NetworkPrefabRef _playerPrefab;
    [SerializeField] private float sensibility = 200f;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    public static BasicSpawner Instance { get; private set; }
    private NetworkRunner _runner;


    private Vector2 absoluteCameraLook;


    private void Update()
    {

        if (InputController.Instance != null && _runner != null && _runner.ProvideInput)
        {
            float lookX = InputController.Instance.GetAxis(InputController.Input.MOUSE_X) * sensibility * Time.deltaTime;
            float lookY = InputController.Instance.GetAxis(InputController.Input.MOUSE_Y) * sensibility * Time.deltaTime;
            absoluteCameraLook.x += lookX;
            absoluteCameraLook.y -= lookY;
            absoluteCameraLook.y = Mathf.Clamp(absoluteCameraLook.y, -80f, 80f);
        }
    }

    void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            Vector3 spawnPosition = new Vector3((player.RawEncoded % runner.Config.Simulation.PlayerCount) * 3, 1, 0);
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
            _spawnedCharacters.Add(player, networkPlayerObject);
        }
    }

    void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
    }

    void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (InputController.Instance == null)
        {
            Debug.LogWarning("InputController.Instance es null");
            return;
        }

        var data = new NetworkInputData();
        float movX = InputController.Instance.GetAxis(InputController.Input.MOVEMENT_X);
        float movY = InputController.Instance.GetAxis(InputController.Input.MOVEMENT_Y);
        data.move = new Vector2(movX, movY);
        data.look = absoluteCameraLook;
        data.buttons.Set(InputButton.jump, InputController.Instance.GetButton(InputController.Input.JUMP));
        input.Set(data);
    }

    void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner) { }
    void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner) { }
    void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner) { }
    void INetworkRunnerCallbacks.OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    void INetworkRunnerCallbacks.OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ReadOnlySpan<byte> data) { }
    void INetworkRunnerCallbacks.OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    async void StartGame(GameMode mode, string roomCode)
    {
        Debug.Log(roomCode);
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        var scene = SceneRef.FromIndex(sceneIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }

        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = roomCode,
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    private string GenerateRoomCode()
    {
        string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string numbers = "1234567890";
        string code = "";
        for (int i = 0; i < 2; i++)
        {
            code += letters[UnityEngine.Random.Range(0, letters.Length)];
        }
        for (int i = 0; i < 2; i++)
        {
            code += numbers[UnityEngine.Random.Range(0, numbers.Length)];
        }
        return code;
    }

    public void HostGame()
    {
        string code = GenerateRoomCode();
        StartGame(GameMode.Host, code);
    }

    public void JoinGame(string code)
    {
        StartGame(GameMode.Client, code);
    }
}