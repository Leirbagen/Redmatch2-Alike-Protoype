using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image damageOverlay;
    [SerializeField] private float flashDuration = 1f;
    [SerializeField] private float maxAlpha = 0.4f;

    private int lastHealth = -1;
    private Coroutine fadeCoroutine;

    private void OnEnable()
    {
        PlayerHealth.OnHealthChange += UpdateHealth;
    }

    private void OnDisable()
    {
        PlayerHealth.OnHealthChange -= UpdateHealth;
    }

    public void UpdateHealth(int currentHealth)
    {
        if (healthText != null)
        {
            healthText.text = currentHealth.ToString();
        }

        if (lastHealth != -1 && currentHealth < lastHealth)
        {
            if (damageOverlay != null)
            {
                if (fadeCoroutine != null)
                {
                    StopCoroutine(fadeCoroutine);
                }
                fadeCoroutine = StartCoroutine(FadeDamageOverlay());
            }
        }

        lastHealth = currentHealth;
    }

    private IEnumerator FadeDamageOverlay()
    {
        if (damageOverlay == null) yield break;
        Color c = damageOverlay.color;
        c.a = maxAlpha;
        damageOverlay.color = c;

        float timer = 0f;
        while (timer < flashDuration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(maxAlpha, 0f, timer / flashDuration);
            damageOverlay.color = c;
            yield return null;
        }
        c.a = 0f;
        damageOverlay.color = c;
    }
}