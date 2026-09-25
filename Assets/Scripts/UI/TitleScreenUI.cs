using UnityEngine;
using UnityEngine.SceneManagement;


public class TitleScreenUI : MonoBehaviour
{
    public void LoadSceneLevel(string levelName) 
    {
        SceneManager.LoadScene(levelName);
    }
}
