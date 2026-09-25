using UnityEngine;
using UnityEngine.SceneManagement;


public class TitleScreenUI : MonoBehaviour
{


    public void LoadLevel(string levelName) 
    {
        SceneManager.LoadScene(levelName);
    }


    private void Customize() 
    {

    }



}
