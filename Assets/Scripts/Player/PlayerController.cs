using UnityEngine;
using Fusion;

public class PlayerController : NetworkBehaviour
{
    private NetworkCharacterController characterController;
    [SerializeField] private float velocity = 5;
    [SerializeField] private float fallDistanceThreshold = 20f;
    [SerializeField] private float damagePerMeter = 2f;
    [SerializeField] private int maxFallDamage = 40;
    private float highestYPosition;
    private bool wasGrounded;
    public AudioSource playerAudio;
    public AudioClip jumpSound;
    public AudioClip landSound;
    public GameObject cameraLook;

    private void Awake()
    {
        characterController = GetComponent<NetworkCharacterController>();
    }

    public override void Spawned()
    {
        base.Spawned();

        if (HasInputAuthority)
        {
            if (cameraLook != null)
            {
                cameraLook.SetActive(true);
                cameraLook.tag = "MainCamera";
            }
        }
        else
        {
            if (cameraLook != null)
            {
                cameraLook.SetActive(false);
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (GetInput(out NetworkInputData data))
        {
            Quaternion yawRotation = Quaternion.Euler(0f, data.look.x, 0f);
            Vector3 cameraForward = yawRotation * Vector3.forward;
            Vector3 cameraRight = yawRotation * Vector3.right;
            Vector3 moveDirection = (cameraRight * data.move.x) + (cameraForward * data.move.y);
            moveDirection.Normalize();

            characterController.Move(moveDirection * velocity * Runner.DeltaTime);
            if (data.buttons.IsSet(InputButton.jump))
            {
                if (characterController.Grounded)
                {
                    characterController.Jump();
                    if (playerAudio != null && jumpSound != null) playerAudio.PlayOneShot(jumpSound);
                }
            }
        }

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
                if (playerAudio != null && landSound != null) playerAudio.PlayOneShot(landSound);
                float fallDistance = highestYPosition - transform.position.y;

                if (fallDistance > fallDistanceThreshold)
                {
                    int calculatedDamage = Mathf.RoundToInt((fallDistance - fallDistanceThreshold) * damagePerMeter);
                    int finalDamage = Mathf.Min(calculatedDamage, maxFallDamage);

                    if (TryGetComponent<IDamageable>(out var damageable))
                    {
                        damageable.TakeDamage(finalDamage);
                    }
                }
            }
            highestYPosition = transform.position.y;
        }
        wasGrounded = characterController.Grounded;
    }
}