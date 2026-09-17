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
            leftGrapple.StopGrapple();
            rightGrapple.StopGrapple();
            if (isJumping == false)
            {
                Jump();
                isJumping = true;
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


