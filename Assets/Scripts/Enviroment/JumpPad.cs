using Fusion;
using UnityEngine;

public class JumpPad : NetworkBehaviour
{
    public float jumpVelocity = 25f;
    public float padRadius = 1.5f;
    public AudioSource padAudio;
    public AudioClip bounceSound;

    public override void FixedUpdateNetwork()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, padRadius);

        foreach (Collider hit in hits)
        {
            PlayerController player = hit.GetComponentInParent<PlayerController>();

            if (player != null)
            {
                if (!player.isPadJumping)
                {
                    player.ApplyJumpPad(jumpVelocity, padAudio, bounceSound);
                }
            }
        }
    }
}