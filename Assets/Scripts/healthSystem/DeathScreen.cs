using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    public void DeathMenuButton(string levelname)
    {
        SceneManager.LoadScene(levelname); 
    }    
    public void QuitGame()
    {
        Application.Quit();
    }

    void Update()
    {

    }
}
