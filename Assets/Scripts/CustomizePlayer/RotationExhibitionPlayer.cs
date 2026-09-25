using Unity.VisualScripting;
using UnityEngine;

public class RotationExhibitionPlayer : MonoBehaviour
{
    private float velocity = 10f;
    private void Update()
    {
        transform.Rotate(Vector3.down * velocity * Time.deltaTime);
    }
}
