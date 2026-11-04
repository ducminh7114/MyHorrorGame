
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinGame : MonoBehaviour
{
    [Header("Main Menu")]
    public string sceneName = "MainMenu";

    // =========================================
    // BACK TO MENU
    // =========================================

    public void BackToMenu()
    {
        Debug.Log("Returning to Main Menu...");

        // Restore normal game settings
        Time.timeScale = 1f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Load Main Menu
        SceneManager.LoadScene(sceneName);
    }

    // =========================================
    // TO DESKTOP
    // =========================================

    public void ToDesktop()
    {
        Debug.Log("Quitting Game...");

        Time.timeScale = 1f;

        Application.Quit();
    }
}