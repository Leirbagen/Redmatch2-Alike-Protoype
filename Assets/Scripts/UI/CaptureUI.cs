using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CaptureUI : MonoBehaviour
{
    [SerializeField] private Slider captureSlider;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private GameObject capturePanel;

    private void OnEnable()
    {
        PlayerController.OnCaptureProgressChanged += UpdateProgress;
    }

    private void OnDisable()
    {
        PlayerController.OnCaptureProgressChanged -= UpdateProgress;
    }

    private void Start()
    {
        if (captureSlider != null)
        {
            captureSlider.minValue = 0f;
            captureSlider.maxValue = 100f;
            captureSlider.value = 0f;
        }
    }

    private void UpdateProgress(float progress)
    {
        if (capturePanel != null && LevelManager.Instance != null)
        {
            bool isKingHill = LevelManager.Instance.IsKingHillMode;
            if (capturePanel.activeSelf != isKingHill)
            {
                capturePanel.SetActive(isKingHill);
            }
        }

        if (captureSlider != null)
        {
            captureSlider.value = progress;
        }

        if (progressText != null)
        {
            progressText.text = Mathf.FloorToInt(progress) + "%";
        }
    }
}