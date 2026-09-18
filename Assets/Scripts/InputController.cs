using UnityEngine;
using Rewired;


public class InputController : MonoBehaviour
{
    public static InputController Instance { get; private set; }
    [SerializeField] private Player input;

    private void Awake()
    {
        if (Instance != null) 
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        input = ReInput.players.GetPlayer(0);
    }
    public float GetAxis(int actionID) 
    {
        return input.GetAxis(actionID);
    }
    public bool GetButtonDown(int actionID) 
    {
        return input.GetButtonDown(actionID);
    }
    public bool GetButton(int actionID)
    {
        return input.GetButton(actionID);
    }
    public bool GetMouseButton(int actionID) 
    {
        return input.GetButtonDown(actionID);
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
    }
}
