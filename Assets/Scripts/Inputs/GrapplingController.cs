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
    public GameObject playerBody;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }
    public void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraLook.position, cameraLook.forward, out hit, maxDistance, grappleableLayer))
        {
            StopGrapple();
            grapplePoint = hit.point;
            joint = playerBody.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;
            float distanceFromPoint = Vector3.Distance(playerBody.transform.position, grapplePoint);
            joint.maxDistance = distanceFromPoint * 0.6f; 
            joint.minDistance = distanceFromPoint * 0.25f;
            joint.spring = 20f;
            joint.damper = 3f;
            joint.massScale = 6f;
            lineRenderer.positionCount = 2;
        }
    }
    public void StopGrapple()
    {
        lineRenderer.positionCount = 0;
        if (joint != null)
        {
            Destroy(joint);
        }
    }
    private void LateUpdate()
    {
        DrawRope();
    }
    private void DrawRope()
    {
        if (!joint) return;
        lineRenderer.SetPosition(0, ropeOut.position);
        lineRenderer.SetPosition(1, grapplePoint);
    }
}
