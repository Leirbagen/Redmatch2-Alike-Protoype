using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class TitleScreenUI : MonoBehaviour
{
    [SerializeField] private GameObject firstSelected;
    [SerializeField] private CanvasGroup mainMenuGroup;
    [SerializeField] private CanvasGroup onlineMenuGroup;

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }
    public void LoadSceneLevel(string levelName) 
    {
        SceneManager.LoadScene(levelName);
    }

    public void SelectButton(GameObject button)
    {
        StartCoroutine(SelectNextFrame(button));
    }

    private IEnumerator SelectNextFrame(GameObject button)
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(button);
    }

    public void SetMainMenuInteractable(bool value)
    {
        mainMenuGroup.interactable = value;
        mainMenuGroup.blocksRaycasts = value;
    }
    public void SetOnlineMenuInteractable(bool value)
    {
        onlineMenuGroup.interactable = value;
        onlineMenuGroup.blocksRaycasts = value;
    }
}
