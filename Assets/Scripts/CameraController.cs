using Unity.Mathematics;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private InputController input;
    [SerializeField] private float sensibility = 35;
    public Transform Player;
    private float XRotation;
    private float YRotation;

    private void Awake()
    {
        input = InputController.Instance;
    }
    private void Start()
    {
        Cursor.visible = false;
    }
    private void Update()
    {
        float ValorX = input.GetAxis(InputController.Input.MOUSE_X) * sensibility * Time.deltaTime;
        float ValorY = input.GetAxis(InputController.Input.MOUSE_Y) * sensibility * Time.deltaTime;
        XRotation += ValorX;
        YRotation -= ValorY;
        YRotation = math.clamp(YRotation, -80, 80);
        transform.localRotation = Quaternion.Euler(YRotation,0f,0f);
        Player.Rotate(Vector3.up * ValorX);
    }
}
