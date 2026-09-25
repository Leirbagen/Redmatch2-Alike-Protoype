using Unity.Mathematics;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private InputController input;
    [SerializeField] private float sensibility = 200;
    public Transform Player;
    private float YRotation;

    private void Start()
    {
        input = InputController.Instance;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        float ValorX = input.GetAxis(InputController.Input.MOUSE_X) * sensibility * Time.deltaTime;
        float ValorY = input.GetAxis(InputController.Input.MOUSE_Y) * sensibility * Time.deltaTime;
        YRotation -= ValorY;
        YRotation = math.clamp(YRotation, -80, 80);
        transform.localRotation = Quaternion.Euler(YRotation,0f,0f);
        Player.Rotate(Vector3.up * ValorX);
    }
}
