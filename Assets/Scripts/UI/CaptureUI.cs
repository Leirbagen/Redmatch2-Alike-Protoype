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
        // Ocultamos la barra si estamos jugando PvP normal
        if (capturePanel != null && BasicSpawner.Instance != null)
        {
            bool isKoth = BasicSpawner.Instance.CurrentGameType == CustomGameType.KingOfTheHill;
            if (capturePanel.activeSelf != isKoth)
            {
                capturePanel.SetActive(isKoth);
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