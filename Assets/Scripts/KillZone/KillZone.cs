using UnityEngine;

public class KillZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth health = other.GetComponent<PlayerHealth>();
        if(health.Object.HasStateAuthority)
        {
            health.TakeDamage(9999);
        }
    }
}
