using UnityEngine;
using Fusion;

public class PlayerController : NetworkBehaviour
{
    private NetworkCharacterController characterController;
    [SerializeField] private float velocity = 5;
    [SerializeField] private float fallDistanceThreshold = 20f;
    [SerializeField] private float damagePerMeter = 2f;
    [SerializeField] private int maxFallDamage = 40;
    private DualGrapplingController grappler;
    [Networked] private float highestYPosition { get; set; }
    [Networked] private bool wasGrounded { get; set; }
    public AudioSource playerAudio;
    public AudioClip jumpSound;
    public AudioClip landSound;
    public GameObject cameraLook;
    [Networked] public float NetworkYaw { get; set; }
    private float pendingJumpForce = 0f;
    public bool isPadJumping = false;

    private void Awake()
    {
        characterController = GetComponent<NetworkCharacterController>();
        grappler = GetComponent<DualGrapplingController>();
    }
    public override void Spawned()
    {
        base.Spawned();

        Camera camComponent = cameraLook.GetComponent<Camera>();
        AudioListener audioListener = cameraLook.GetComponent<AudioListener>();

        if (HasInputAuthority)
        {
            if (camComponent != null)
            {
                camComponent.enabled = true;
            }
            if (audioListener != null)
            {
                audioListener.enabled = true;
            }
            cameraLook.tag = "MainCamera";
        }
        else
        {
            if (camComponent != null)
            {
                camComponent.enabled = false;
            }
            if (audioListener != null)
            {
                audioListener.enabled = false;
            }
        }
        if (Object.HasStateAuthority)
        {
            highestYPosition = transform.position.y;
            wasGrounded = true;
        }
    }
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (!Object.HasStateAuthority && !Object.HasInputAuthority)
        {
            return;
        }
        Vector3 moveDirection = Vector3.zero;
        bool jumpPressed = false;

        if (GetInput(out NetworkInputData data))
        {
            NetworkYaw = data.look.x;
            Quaternion yawRotation = Quaternion.Euler(0f, NetworkYaw, 0f);
            Vector3 cameraForward = yawRotation * Vector3.forward;
            Vector3 cameraRight = yawRotation * Vector3.right;
            moveDirection = (cameraRight * data.move.x) + (cameraForward * data.move.y);
            moveDirection.Normalize();
            jumpPressed = data.buttons.IsSet(InputButton.jump);
        }
        if (pendingJumpForce > 0f)
        {
            if (characterController.Grounded)
            {
                characterController.Jump();
            }
            characterController.Velocity = new Vector3(characterController.Velocity.x, pendingJumpForce, characterController.Velocity.z);
            pendingJumpForce = 0f;
        }
        bool isGrappling = false;
        if (grappler != null)
        {
            isGrappling = grappler.IsGrappling;
        }
        bool isInAir = !characterController.Grounded;
        bool useAirbornePhysics = false;
        if (grappler != null)
        {
            if (isInAir || isGrappling)
            {
                useAirbornePhysics = true;
            }
        }
        if (useAirbornePhysics)
        {
            grappler.SimulateAirborne(moveDirection);
        }
        else
        {
            characterController.Move(moveDirection * velocity * Runner.DeltaTime);
        }

        if (jumpPressed && characterController.Grounded)
        {
            characterController.Jump();
            if (Runner.IsForward)
            {
                if (playerAudio != null && jumpSound != null)
                {
                    playerAudio.PlayOneShot(jumpSound);
                }
            }

        }


        transform.rotation = Quaternion.Euler(0f, NetworkYaw, 0f);
        if (!characterController.Grounded)
        {
            if (transform.position.y > highestYPosition)
            {
                highestYPosition = transform.position.y;
            }
        }
        else
        {
            if (!wasGrounded)
            {
                if (Runner.IsForward)
                {
                    if (playerAudio != null && landSound != null)
                    {
                        playerAudio.PlayOneShot(landSound);
                    }
                }
                float fallDistance = highestYPosition - transform.position.y;
                if (fallDistance > fallDistanceThreshold && !isPadJumping && Object.HasStateAuthority)
                {
                    int calculatedDamage = Mathf.RoundToInt((fallDistance - fallDistanceThreshold) * damagePerMeter);
                    int finalDamage = Mathf.Min(calculatedDamage, maxFallDamage);

                    if (TryGetComponent<IDamageable>(out var damageable))
                    {
                        damageable.TakeDamage(finalDamage);
                    }
                }

                isPadJumping = false; 

            }
            highestYPosition = transform.position.y;
        }
        wasGrounded = characterController.Grounded;
    }

    public void ApplyJumpPad(float force)
    {
        pendingJumpForce = force;
        isPadJumping = true;
    }

}