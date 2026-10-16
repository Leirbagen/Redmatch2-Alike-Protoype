using System;
using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TextMeshProUGUI labelText;

    [Header("Pantalla de Victoria")]
    public GameObject winnerPanel; 
    public TextMeshProUGUI winnerText; 

    private void OnEnable()
    {
        LevelManager.OnTimeUpdated += UpdateTimeUI;
        LevelManager.OnMatchEnded += ShowWinnerUI;
    }

    private void OnDisable()
    {
        LevelManager.OnTimeUpdated -= UpdateTimeUI;
        LevelManager.OnMatchEnded -= ShowWinnerUI;
    }

    private void UpdateTimeUI(float timeRemaining)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(Mathf.Max(0, timeRemaining));
        timeText.text = string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
    }

    private void ShowWinnerUI(string winnerName, int kills)
    {
        timeText.gameObject.SetActive(false);
        labelText.gameObject.SetActive(false);

        if (winnerPanel != null && winnerText != null)
        {
            winnerPanel.SetActive(true);
            winnerText.text = $"¡GANADOR:\n{winnerName}!\n\nBajas: {kills}";
        }
    }
}