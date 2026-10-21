using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HitmarkerUI : MonoBehaviour
{
    [SerializeField] private Image hitmarkerImage;
    [SerializeField] private float displayDuration = 0.15f; 
    [SerializeField] private AudioSource hitAudioSource;
    [SerializeField] private AudioClip hitSound;

    private Coroutine flashCoroutine;

    private void OnEnable()
    {
        HitWeapon.OnTargetHit += ShowHitmarker;
    }

    private void OnDisable()
    {
        HitWeapon.OnTargetHit -= ShowHitmarker;
    }

    private void Start()
    {
        if (hitmarkerImage != null)
        {
            hitmarkerImage.enabled = false;
        }
    }

    private void ShowHitmarker()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(FlashRoutine());
        if (hitAudioSource != null && hitSound != null)
        {
            hitAudioSource.PlayOneShot(hitSound);
        }
    }

    private IEnumerator FlashRoutine()
    {
        hitmarkerImage.enabled = true;
        yield return new WaitForSeconds(displayDuration);
        hitmarkerImage.enabled = false;
    }
}