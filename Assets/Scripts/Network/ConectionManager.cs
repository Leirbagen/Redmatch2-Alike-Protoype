using UnityEngine;
using TMPro;

public class ConectionManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField codeInput;

    [Header("UI Panels")]
    [SerializeField] private GameObject gameModePanel; 
    private CustomGameType selectedMode = CustomGameType.PvP;

    public void OpenGameModePanel()
    {
        if (gameModePanel != null)
        {
            gameModePanel.SetActive(true);
        }
    }

    public void SelectPvPMode()
    {
        selectedMode = CustomGameType.PvP;
        Debug.Log("Deathmatch PvP");

        if (gameModePanel != null)
        {
            gameModePanel.SetActive(false); 
        }
    }

    public void SelectKingOfTheHillMode()
    {
        selectedMode = CustomGameType.KingOfTheHill;
        Debug.Log("King of the Hill");

        if (gameModePanel != null)
        {
            gameModePanel.SetActive(false); 
        }
    }
    public void Host(int sceneIndex)
    {
        BasicSpawner.Instance.HostGame(sceneIndex, selectedMode);
    }

    public void Join()
    {
        string code = codeInput.text.Trim().ToUpper();
        BasicSpawner.Instance.JoinGame(code);
    }
}