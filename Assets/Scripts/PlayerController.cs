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
    [SerializeField] private GrapplingController grappling;
    private bool isGrappling = false;
    private InputController input;

    private void Awake()
    {
        input = InputController.Instance;
    }
    private void Start()
    {
        myBody = GetComponent<Rigidbody>();
    }
    private void MovePlayer() 
    {
        float movX = input.GetAxis(InputController.Input.MOVEMENT_X);
        float movZ = input.GetAxis(InputController.Input.MOVEMENT_Y);
        Vector3 inputMovement = (transform.right * movX) + (transform.forward * movZ);
        Vector3 finalVelocity = inputMovement * velocity;
        finalVelocity.y = myBody.linearVelocity.y;
        myBody.linearVelocity = finalVelocity;
    }
    private void Jump() 
    {
        myBody.AddForce(Vector3.up * forceJump, ForceMode.Impulse);
    }
    private void Update()
    {
        MovePlayer();

        if (input.GetButtonDown(InputController.Input.JUMP))
        {
            if (isJumping == false)
            {
                Jump();
                isJumping = true;
            }
            else if (isJumping == true && isGrappling == false)
            {
                grappling.StartGrapple();
                isGrappling = true;
            }
            else if (isGrappling == true)
            {
                grappling.StopGrapple();
                isGrappling = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isJumping = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            isJumping = true;
        }
    }
}


