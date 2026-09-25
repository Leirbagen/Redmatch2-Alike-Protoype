using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using Rewired;

public class PlayerController : MonoBehaviour
{
    private Rigidbody myBody;
    [SerializeField] private float velocity = 5;
    [SerializeField] private float forceJump = 5;
    [SerializeField] private bool isJumping = false;
    [SerializeField] private GrapplingController leftGrapple;
    [SerializeField] private GrapplingController rightGrapple;
    [SerializeField] private float groundCheckDistance = 1.1f;
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
            Vector3 finalVelocity = inputMovement * velocity;
            finalVelocity.y = myBody.linearVelocity.y;
            myBody.linearVelocity = finalVelocity;
        }
        else
        {
            bool isGrappling = GetComponent<SpringJoint>() != null;

            if (isGrappling)
            {
                myBody.AddForce(inputMovement * (velocity * 0.5f), ForceMode.Acceleration);
            }
            else
            {
                Vector3 currentFlatVelocity = new Vector3(myBody.linearVelocity.x, 0f, myBody.linearVelocity.z);
                Vector3 targetFlatVelocity = inputMovement * velocity;
                Vector3 newFlatVelocity = Vector3.MoveTowards(currentFlatVelocity, targetFlatVelocity, (velocity * 8f) * Time.fixedDeltaTime);
                myBody.linearVelocity = new Vector3(newFlatVelocity.x, myBody.linearVelocity.y, newFlatVelocity.z);
            }
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
        if (isJumping && touchingGround && myBody.linearVelocity.y <= -0.1f)
        {
            playerAudio.PlayOneShot(landSound);
        }
        isJumping = !touchingGround;
        if (input.GetButtonDown(InputController.Input.JUMP))
        {
            leftGrapple.StopGrapple();
            rightGrapple.StopGrapple();
            if (isJumping == false)
            {
                Jump();
                isJumping = true;
                playerAudio.PlayOneShot(jumpSound);
            }
        }
        if (input.GetButtonDown(InputController.Input.GRAPPLE_LEFT))
        {
            leftGrapple.StartGrapple();
        }
        if (input.GetButtonDown(InputController.Input.GRAPPLE_RIGHT))
        {
            rightGrapple.StartGrapple();
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }
}


