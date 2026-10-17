using UnityEngine;
using Rewired;
using System.Collections.Generic;

public enum InputMode {
    Gameplay,
    Menu
}

public class InputController : MonoBehaviour
{
    public int playerId = 0;
    public static InputController Instance { get; private set; }
    [SerializeField] private InputMode startingMode = InputMode.Menu;
    private InputMode currentGameMode;
    private Player player;
    static Dictionary<InputMode, string> gameModeToMapCategory = new Dictionary<InputMode, string>() {
        { InputMode.Gameplay, "inGame" },
        { InputMode.Menu, "inMenu" },
    };

    private void Awake()
    {
        if (Instance != null) 
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        player = ReInput.players.GetPlayer(playerId);
        player.controllers.ControllerAddedEvent += OnControllerAdded;
        ChangeGameMode(startingMode);
    }

    void OnDestroy() {
        if (Instance != this)
        {
            return; 
        } 
        if (ReInput.isReady && player != null)
        {
            player.controllers.ControllerAddedEvent -= OnControllerAdded;
        }
    }

    public void ChangeGameMode(InputMode mode) {
        currentGameMode = mode; // store the new game mode
        SetControllerMapsForCurrentGameMode(); // enable the correct Controller Maps for the game mode
    }

    void OnControllerAdded(ControllerAssignmentChangedEventArgs args) {
        SetControllerMapsForCurrentGameMode();
    }

    void SetControllerMapsForCurrentGameMode() {
        // Disable all controller maps first for all controllers of all types
        player.controllers.maps.SetAllMapsEnabled(false);

        // Enable maps for the current game mode for all controlllers of all types
        player.controllers.maps.SetMapsEnabled(true, gameModeToMapCategory[currentGameMode]);
    }

    public float GetAxis(int actionID) 
    {
        return player.GetAxis(actionID);
    }
    public bool GetButtonDown(int actionID) 
    {
        return player.GetButtonDown(actionID);
    }
    public bool GetButton(int actionID)
    {
        return player.GetButton(actionID);
    }
    public bool GetMouseButton(int actionID) 
    {
        return player.GetButtonDown(actionID);
    }
    public static class Input 
    {
        public const int MOVEMENT_Y = 0;
        public const int MOVEMENT_X = 1;
        public const int JUMP = 2;
        public const int MOUSE_X = 3;
        public const int MOUSE_Y = 4;
        public const int GRAPPLE_LEFT = 8;
        public const int GRAPPLE_RIGHT = 9;
        public const int FIRE_1 = 10;
        public const int RELOAD_WEAPON = 11;
        public const int SCROLL_WHEEL = 12;
        public const int AIM_WEAPON = 13;
        ///////MENU////////
        public const int MENUVERTICAL = 14;
        public const int MENUHORIZONTAL = 15;
        public const int MENUSUBMIT = 16;
        public const int MENUCANCEL = 17;
    }
}
