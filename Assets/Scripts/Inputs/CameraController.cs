using Fusion;
using Unity.Mathematics;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private float sensibility = 200f;
    public Transform playerBody;
    [Networked] private float pitchRotation { get; set; }
    [Networked] private float yawRotation { get; set; }

    public override void Spawned()
    {
        if (HasInputAuthority)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            float lookX = data.look.x * sensibility * Runner.DeltaTime;
            float lookY = data.look.y * sensibility * Runner.DeltaTime;

            yawRotation += lookX;
            pitchRotation -= lookY;
            pitchRotation = math.clamp(pitchRotation, -80, 80);

            if (playerBody != null)
            {
                playerBody.rotation = Quaternion.Euler(0f, yawRotation, 0f);
            }
            transform.localRotation = Quaternion.Euler(pitchRotation, 0f, 0f);
        }
    }
}