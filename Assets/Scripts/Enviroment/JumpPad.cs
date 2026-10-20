using Fusion;
using UnityEngine;

// Ahora la plataforma funciona dentro de la red de Photon
public class JumpPad : NetworkBehaviour
{
    [Header("Configuración del Salto")]
    public float jumpVelocity = 25f;
    public float padRadius = 1.5f; 

    [Header("Audio")]
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
                    player.ApplyJumpPad(jumpVelocity);

                    if (Runner.IsForward && padAudio != null && bounceSound != null)
                    {
                        padAudio.PlayOneShot(bounceSound);
                    }
                }
            }
        }
    }

}