using Fusion;
using UnityEngine;
using System.Collections.Generic;

public enum CaptureState
{
    Neutral,
    Capturing,
    Contested
}

public class CaptureZone : NetworkBehaviour
{
    public float zoneRadius = 5f;
    public float pointsPerSecond = 60f;
    public MeshRenderer zoneRenderer;
    public Color neutralColor = new Color(1f, 1f, 1f, 0.2f);
    public Color capturingColor = new Color(0f, 1f, 0f, 0.4f);
    public Color contestedColor = new Color(1f, 0f, 0f, 0.4f);
    [Networked]
    public CaptureState CurrentState { get; set; }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
        {
            return;
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, zoneRadius);
        List<PlayerController> playersInside = new List<PlayerController>();
        foreach (Collider hit in hits)
        {
            PlayerController player = hit.GetComponentInParent<PlayerController>();
            if (player != null && !playersInside.Contains(player))
            {
                PlayerHealth health = player.GetComponent<PlayerHealth>();
                if (health != null && !health.isDead)
                {
                    playersInside.Add(player);
                }
            }
        }

        if (playersInside.Count == 1)
        {
            playersInside[0].AddCaptureProgress(pointsPerSecond * Runner.DeltaTime);
            CurrentState = CaptureState.Capturing;
        }
        else if (playersInside.Count > 1)
        {
            CurrentState = CaptureState.Contested;
        }
        else
        {
            CurrentState = CaptureState.Neutral;
        }
    }

    public override void Render()
    {
        if (zoneRenderer == null)
        {
            return;
        }

        Color targetColor = neutralColor;
        switch (CurrentState)
        {
            case CaptureState.Capturing:
                targetColor = capturingColor;
                break;
            case CaptureState.Contested:
                targetColor = contestedColor;
                break;
            case CaptureState.Neutral:
                targetColor = neutralColor;
                break;
        }

        zoneRenderer.material.color = Color.Lerp(zoneRenderer.material.color, targetColor, Time.deltaTime * 5f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, zoneRadius);
    }
}
