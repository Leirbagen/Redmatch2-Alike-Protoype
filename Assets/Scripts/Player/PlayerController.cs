using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using Rewired;
using Fusion;

public class PlayerController : NetworkBehaviour
{
    private NetworkCharacterController characterController;
    [SerializeField] private float velocity = 5;
    [SerializeField] private float forceJump = 5;
    [SerializeField] private bool isJumping = false;
    [SerializeField] private GrapplingController leftGrapple;
    [SerializeField] private GrapplingController rightGrapple;
    public bool isSwinging => leftGrapple.IsHooked || rightGrapple.IsHooked;
    [SerializeField] private float swingControlForce = 20f;
    [SerializeField] private float groundCheckDistance = 1.1f;
    [SerializeField] private float fallDistanceThreshold = 20f;
    [SerializeField] private float damagePerMeter = 2f;
    [SerializeField] private int maxFallDamage = 40;
    [SerializeField] private float airControlForce = 15f;
    private bool leftWaitRelease;
    private bool rightWaitRelease;
    private float highestYPosition;
    private InputController input;
    public AudioSource playerAudio;
    public AudioClip jumpSound;
    public AudioClip landSound;

    private void Start()
    {
        myBody = GetComponent<Rigidbody>();
        input = InputController.Instance;
    }
    private void MovePlayer()
    {
        float movX = input.GetAxis(InputController.Input.MOVEMENT_X);
        float movZ = input.GetAxis(InputController.Input.MOVEMENT_Y);
        Vector3 inputMovement = (transform.right * movX) + (transform.forward * movZ);
        if (isJumping == false)
        {
            Vector3 finalVelocity = inputMovement.normalized * velocity;
            finalVelocity.y = myBody.linearVelocity.y;
            myBody.linearVelocity = finalVelocity;
        }
        else if (isSwinging)
        {
            myBody.AddForce(inputMovement * swingControlForce, ForceMode.Acceleration);
        }
        else
        {
            myBody.AddForce(inputMovement * airControlForce, ForceMode.Acceleration);
        }
    }
    private void Jump()
    {
        myBody.AddForce(Vector3.up * forceJump, ForceMode.Impulse);
    }
    private void Update()
    {
        bool touchingGround = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
        Debug.DrawRay(transform.position, Vector3.down * groundCheckDistance, Color.red);

        if (!touchingGround)
        {
            if (transform.position.y > highestYPosition)
            {
                highestYPosition = transform.position.y;
            }
        }
        if (isSwinging) 
        {
            highestYPosition = transform.position.y;
        }
        if (isJumping && touchingGround)
        {
            playerAudio.PlayOneShot(landSound);
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
        if (touchingGround)
        {
            highestYPosition = transform.position.y;
        }
        isJumping = !touchingGround;
        if (input.GetButtonDown(InputController.Input.JUMP))
        {
            leftGrapple.StopGrapple();
            rightGrapple.StopGrapple();
            leftWaitRelease = true;
            rightWaitRelease = true;
            if (isJumping == false)
            {
                Jump();
                playerAudio.PlayOneShot(jumpSound);
            }
        }
        HandleGrapples();
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void HandleGrapples()
    {
        if (input.GetButton(InputController.Input.GRAPPLE_LEFT) && !leftWaitRelease)
        {
            leftGrapple.StartGrapple();
        }
        else
        {
            leftWaitRelease = false;
            leftGrapple.StopGrapple();
        }
        if (input.GetButton(InputController.Input.GRAPPLE_RIGHT) && !rightWaitRelease)
        {
            rightGrapple.StartGrapple();
        }
        else
        {
            rightWaitRelease = false;
            rightGrapple.StopGrapple();
        }
    }
}