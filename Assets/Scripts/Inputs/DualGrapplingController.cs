using Fusion;
using UnityEngine;

[DefaultExecutionOrder(-100)]
[RequireComponent(typeof(NetworkCharacterController))]
public class DualGrapplingController : NetworkBehaviour
{
    public float maxDistance = 100f;
    public LayerMask grappleableLayer;
    public float eyeHeight = 1.5f;
    public float maxLengthFactor = 0.3f;
    public float minLengthFactor = 0.25f;
    public float spring = 4.5f;
    public float damper = 7f;
    public float springForceMultiplier = 1f;
    public float swingControlForce = 20f;
    public float airControlForce = 15f;
    public float grappleLaunchUpVelocity = 4f;
    public Transform cameraLook;
    public Transform leftRopeOut;
    public LineRenderer leftLineRenderer;
    public AudioSource leftAudio;
    public Transform rightRopeOut;
    public LineRenderer rightLineRenderer;
    public AudioSource rightAudio;
    public AudioClip shootSound;
    [Networked] public NetworkBool IsLeftHooked { get; set; }
    [Networked] public Vector3 LeftGrapplePoint { get; set; }
    [Networked] private float LeftMaxLength { get; set; }
    [Networked] private float LeftMinLength { get; set; }
    [Networked] public NetworkBool IsRightHooked { get; set; }
    [Networked] public Vector3 RightGrapplePoint { get; set; }
    [Networked] private float RightMaxLength { get; set; }
    [Networked] private float RightMinLength { get; set; }
    [Networked] private NetworkButtons PreviousButtons { get; set; }
    public bool IsGrappling => IsLeftHooked || IsRightHooked;
    private NetworkCharacterController characterController;
    private CharacterController unityController;

    private void Awake()
    {
        characterController = GetComponent<NetworkCharacterController>();
        unityController = GetComponent<CharacterController>();
        if (leftLineRenderer != null)
        {
            leftLineRenderer.enabled = false;
        } 
        if (rightLineRenderer != null)
        {
            rightLineRenderer.enabled = false;
        }
    } 

    public override void FixedUpdateNetwork()
    {
        if (!GetInput(out NetworkInputData data))
        {
            return;
        }
        bool leftPressed = data.buttons.WasPressed(PreviousButtons, InputButton.grappleLeft);
        bool leftReleased = data.buttons.WasReleased(PreviousButtons, InputButton.grappleLeft);
        bool rightPressed = data.buttons.WasPressed(PreviousButtons, InputButton.grappleRight);
        bool rightReleased = data.buttons.WasReleased(PreviousButtons, InputButton.grappleRight);
        PreviousButtons = data.buttons;

        HandleGrappleAction(leftPressed, leftReleased, true, data);
        HandleGrappleAction(rightPressed, rightReleased, false, data);
        
    }

    private void HandleGrappleAction(bool pressed, bool released, bool isLeft, NetworkInputData data)
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
            Vector3 rayOrigin = transform.position + (Vector3.up * eyeHeight);
            Quaternion aimRotation = Quaternion.Euler(data.look.y, data.look.x, 0);
            Vector3 aimDirection = aimRotation * Vector3.forward;

            if (Physics.Raycast(rayOrigin, aimDirection, out RaycastHit hit, maxDistance, grappleableLayer))
            {
                if (Runner.IsForward)
                {
                    if (isLeft && leftAudio != null)
                    {
                        leftAudio.PlayOneShot(shootSound);
                    } 
                    else if (!isLeft && rightAudio != null)
                    {
                        rightAudio.PlayOneShot(shootSound);
                    } 
                }
                float distance = Vector3.Distance(transform.position, hit.point);

                if (isLeft)
                {
                    IsLeftHooked = true;
                    LeftGrapplePoint = hit.point;
                    LeftMaxLength = distance * maxLengthFactor;
                    LeftMinLength = distance * minLengthFactor;
                }
                else
                {
                    IsRightHooked = true;
                    RightGrapplePoint = hit.point;
                    RightMaxLength = distance * maxLengthFactor;
                    RightMinLength = distance * minLengthFactor;
                }

                if (characterController.Grounded)
                {
                    characterController.Velocity = new Vector3(characterController.Velocity.x, grappleLaunchUpVelocity, characterController.Velocity.z);
                }
            }
        }
    }

    public void SimulateAirborne(Vector3 moveDirection)
    {
        float dt = Runner.DeltaTime;
        Vector3 velocity = characterController.Velocity;
        if (characterController.Grounded && velocity.y < 0f)
        {
            velocity.y = 0f;
        } 
        velocity.y += characterController.gravity * dt;

        if (IsLeftHooked)
        {
            velocity += SpringAcceleration(LeftGrapplePoint, LeftMaxLength, LeftMinLength, velocity) * dt;
        }
        if (IsRightHooked)
        {
            velocity += SpringAcceleration(RightGrapplePoint, RightMaxLength, RightMinLength, velocity) * dt;
        }

        float control;
        if (IsGrappling)
        {
            control = swingControlForce;
        }
        else
        {
            control = airControlForce;
        }
        velocity += moveDirection * control * dt;

        Vector3 before = transform.position;
        unityController.Move(velocity * dt);
        characterController.Grounded = unityController.isGrounded;
        characterController.Velocity = (transform.position - before) / dt;
    }

    private Vector3 SpringAcceleration(Vector3 point, float maxLen, float minLen, Vector3 velocity)
    {
        Vector3 toPoint = point - transform.position;
        float dist = toPoint.magnitude;
        if (dist < 0.001f)
        {
            return Vector3.zero;
        } 
        Vector3 dir = toPoint / dist;

        float stretch;
        if (dist > maxLen)
        {
            stretch = dist - maxLen;  
        }
        else if (dist < minLen)
        {
            stretch = dist - minLen;
        }
        else
        {
            return Vector3.zero;
        }
        float radialVelocity = Vector3.Dot(velocity, dir);

        return dir * (spring * springForceMultiplier * stretch - damper * radialVelocity);
    }

/*
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
*/
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