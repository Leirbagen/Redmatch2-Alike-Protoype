using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color exhaustedColor = Color.red;
    [SerializeField] private float smoothSpeed = 15f;
    private PlayerController player;

    public void Bind(PlayerController newPlayer)
    {
        player = newPlayer;
    }

    private void Update()
    {
        if (player == null)
        {
            canvasGroup.alpha = 0f;
            return;
        }

        float target = player.StaminaNormalized;
        fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, target, smoothSpeed * Time.deltaTime);
        if (player.IsExhausted)
        {
            fillImage.color = exhaustedColor;
        }
        else
        {
            fillImage.color = normalColor;
        }

        float targetAlpha;
        if(target >= 0.99f)
        {
            targetAlpha = 0f;
        }
        else
        {
            targetAlpha = 1f;
        }
        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, 8f * Time.deltaTime);
    }
}
