using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenuUI : MonoBehaviour
{
    /*
    public GameObject deathPanel;

    private void OnEnable()
    {
        PlayerHealth.OnPlayerDied += ShowDeathPanel;
        LevelManager.OnTimeOutLost += ShowDeathPanel;
    }
    private void OnDisable()
    {
        PlayerHealth.OnPlayerDied -= ShowDeathPanel;
        LevelManager.OnTimeOutLost -= ShowDeathPanel;
    }
    public void ShowDeathPanel()
    {
        deathPanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void ReloadLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScreen");
    }
    */
}