using Fusion;
using UnityEngine;

[RequireComponent(typeof(NetworkCharacterController))]
public class DualGrapplingController : NetworkBehaviour
{
    public float maxDistance = 100f; 
    public LayerMask grappleableLayer; 
    public Transform cameraLook; 
    public float pullForce = 25f;
    public float swingSpeedBoost = 15f;
    public Transform leftRopeOut;
    public LineRenderer leftLineRenderer;
    public AudioSource leftAudio;
    public Transform rightRopeOut;
    public LineRenderer rightLineRenderer;
    public AudioSource rightAudio;

    public AudioClip shootSound; 

    [Networked] public NetworkBool IsLeftHooked { get; set; }
    [Networked] public Vector3 LeftGrapplePoint { get; set; }
    [Networked] private float LeftDistance { get; set; }

    [Networked] public NetworkBool IsRightHooked { get; set; }
    [Networked] public Vector3 RightGrapplePoint { get; set; }
    [Networked] private float RightDistance { get; set; }

    [Networked] private NetworkButtons PreviousButtons { get; set; }
    private NetworkCharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<NetworkCharacterController>();
        leftLineRenderer.enabled = false;
        rightLineRenderer.enabled = false;
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            bool leftPressed = data.buttons.WasPressed(PreviousButtons, InputButton.grappleLeft);
            bool leftReleased = data.buttons.WasReleased(PreviousButtons, InputButton.grappleLeft);
            bool rightPressed = data.buttons.WasPressed(PreviousButtons, InputButton.grappleRight);
            bool rightReleased = data.buttons.WasReleased(PreviousButtons, InputButton.grappleRight);
            PreviousButtons = data.buttons;

            HandleGrappleAction(leftPressed, leftReleased, true);
            HandleGrappleAction(rightPressed, rightReleased, false);

            if (IsLeftHooked || IsRightHooked)
            {
                Vector3 currentVelocity = characterController.Velocity;

                if (IsLeftHooked) 
                {
                    ApplyGrapplePhysics(LeftGrapplePoint, LeftDistance, ref currentVelocity);

                }
                if (IsRightHooked) 
                {
                    ApplyGrapplePhysics(RightGrapplePoint, RightDistance, ref currentVelocity);

                }
                characterController.Velocity = currentVelocity;
            }
        }
    }
    private void HandleGrappleAction(bool pressed, bool released, bool isLeft)
    {
        if (released)
        {
            if (isLeft)
            {
                IsLeftHooked = false;
            }
            else 
            {
                IsRightHooked = false;
            } 
            return;
        }

        if (pressed)
        {
            if (Physics.Raycast(cameraLook.position, cameraLook.forward, out RaycastHit hit, maxDistance, grappleableLayer))
            {
                if (isLeft && leftAudio != null)
                {
                    leftAudio.PlayOneShot(shootSound);
                }
                else if  (!isLeft && rightAudio != null) rightAudio.PlayOneShot(shootSound);

                if (isLeft)
                {
                    IsLeftHooked = true;
                    LeftGrapplePoint = hit.point;
                    LeftDistance = Vector3.Distance(transform.position, LeftGrapplePoint);
                }
                else
                {
                    IsRightHooked = true;
                    RightGrapplePoint = hit.point;
                    RightDistance = Vector3.Distance(transform.position, RightGrapplePoint);
                }
            }
        }
    }

    private void ApplyGrapplePhysics(Vector3 point, float ropeLength, ref Vector3 velocity)
    {
        Vector3 currentPos = transform.position;
        Vector3 dirToPoint = (point - currentPos).normalized;
        float currentDist = Vector3.Distance(currentPos, point);

        if (currentDist > ropeLength)
        {
            float velocityAway = Vector3.Dot(velocity, dirToPoint);
            if (velocityAway < 0)
            {
                velocity -= velocityAway * dirToPoint;
                velocity += velocity.normalized * swingSpeedBoost * Runner.DeltaTime;
            }
        }
        velocity += dirToPoint * pullForce * Runner.DeltaTime;
    }

    public void LateUpdate()
    {
        DrawRope(leftLineRenderer, leftRopeOut, IsLeftHooked, LeftGrapplePoint);
        DrawRope(rightLineRenderer, rightRopeOut, IsRightHooked, RightGrapplePoint);
    }

    private void DrawRope(LineRenderer line, Transform ropeOut, bool hooked, Vector3 point) 
    {
        if (line == null || ropeOut == null) return;

        line.enabled = hooked;
        if (hooked)
        {
            line.SetPosition(0, ropeOut.position); 
            line.SetPosition(1, point); 
        }
    }
}