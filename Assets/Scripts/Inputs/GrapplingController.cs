using System.Runtime.CompilerServices;
using UnityEngine;

public class GrapplingController : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private SpringJoint joint;
    private Vector3 grapplePoint;
    public Transform cameraLook;
    public Transform ropeOut;
    public float maxDistance = 100f;
    public LayerMask grappleableLayer;
    public Transform player; //instead of playerBody
    [SerializeField] private bool isHooked;
    //[SerializeField] private int inputAction;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip shootSound;
    public bool IsHooked => isHooked;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }
/*
    private void Update()
    {
        if(InputController.Instance.GetButton(inputAction))
        {
            if (!isHooked)
            {
                StartGrapple();
            }
        }
        else
        {
            if(isHooked)
            {
                StopGrapple();
            }
        }
    }
*/
    private void LateUpdate()
    {
        DrawRope();
    }
    public void StartGrapple()
    {
        if(isHooked)
        {
            return;
        }
        RaycastHit hit;
        if(Physics.Raycast(cameraLook.position, cameraLook.forward, out hit, maxDistance, grappleableLayer))
        {
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            joint.maxDistance = distanceFromPoint * 0.3f;
            joint.minDistance = distanceFromPoint * 0.25f;

            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            lineRenderer.positionCount = 2;
            isHooked = true;
            
            audioSource.PlayOneShot(shootSound);
        }
    }
    public void StopGrapple()
    {
        if(!isHooked)
        {
            return;
        }
        lineRenderer.positionCount = 0;
        Destroy(joint);
        isHooked = false;
        
    }
   
    private void DrawRope()
    {
        if(!joint)
        {
            return;
        }
        lineRenderer.SetPosition(0, ropeOut.position);
        lineRenderer.SetPosition(1, grapplePoint);
    }
}
