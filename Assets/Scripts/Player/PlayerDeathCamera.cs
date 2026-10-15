using System.Collections;
using UnityEngine;

public class PlayerDeathCamera : MonoBehaviour
{
    public static PlayerDeathCamera Instance { get; private set; }
    [SerializeField] private Camera cam;
    [SerializeField] private AudioListener lisetner;
    [SerializeField] private float riseHeight;
    [SerializeField] private float pullBack;
    [SerializeField] private float lookDown;
    [SerializeField] private float duration = 2.5f;
    private Coroutine routine;

    private void Awake()
    {
        Instance = this;
        SetVisible(false);
    }

    private void OnDestroy()
    {
        if(Instance == this)
        {
            Instance = null;
        }
    }

    private void SetVisible(bool isVisible)
    {
        cam.enabled = isVisible;
        lisetner.enabled = isVisible;
    }

    public void Play(Vector3 startPosition, Quaternion startRotation)
    {
        transform.SetPositionAndRotation(startPosition, startRotation);
        SetVisible(true);

        if (routine != null)
        {
            StopCoroutine(routine);
        } 
        routine = StartCoroutine(Animate(startPosition, startRotation));
    }

    public void Stop()
    {
        if(routine != null)
        {
            StopCoroutine(routine);
            routine = null;
            SetVisible(false);
        }
    }

    private IEnumerator Animate(Vector3 startPos, Quaternion startRot)
    {
        Vector3 endPos = startPos + Vector3.up * riseHeight - startRot * Vector3.forward * pullBack;
        Quaternion endRot = Quaternion.Euler(lookDown, startRot.eulerAngles.y, 0f);
        float elapsed = 0f;

        while(elapsed < duration)
        {
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            transform.position = Vector3.Lerp(startPos, endPos, t);
            transform.rotation = Quaternion.Slerp(startRot, endRot, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.SetPositionAndRotation(endPos, endRot);
    }
}
