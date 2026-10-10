using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;   

public class PauseGame : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject settingsMenu;

    public string sceneName;
    public bool toggle;
    public playerMovement playerScript;

    public bool canPause = true;

    void Update()
    {
        if (!canPause)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            toggle = !toggle;
            if (toggle == false)
            {
                pauseMenu.SetActive(false);
                Time.timeScale = 1f;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                playerScript.enabled = true;
            }
            if (toggle == true)
            {
                pauseMenu.SetActive(true);
                Time.timeScale = 0f;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                playerScript.enabled = false;
            }
        }
    }
    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        playerScript.enabled = true;
        toggle = false;
        Debug.Log("Resume game");
    }
    public void toSettings()
    {
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(true);
        Debug.Log("Open Settings menu");
    }
    public void backToPause()
    {
        settingsMenu.SetActive(false);
        pauseMenu.SetActive(true);
        Debug.Log("Back to pause menu");
    }
    public void QuitToMainMenu()
    {
        Debug.Log("=================================");
        Debug.Log("QUITTING TO MAIN MENU");
        Debug.Log("=================================");

        if (GameSaveManager.Instance != null)
        {
            Debug.Log(
                "HasSaveGame BEFORE MENU = " +
                GameSaveManager.Instance.HasSaveGame()
            );

            Debug.Log(
                "Saved Scene = " +
                GameSaveManager.Instance.GetSavedScene()
            );

            GameSaveManager.Instance.DebugSave();
        }
        else
        {
            Debug.LogError(
                "GameSaveManager not found!"
            );
        }

        Time.timeScale = 1f;

        UnityEngine.SceneManagement.SceneManager.LoadScene(
            "MainMenu"
        );
    }
    public void QuitGame()
    {
        Debug.Log("This will quit the game in the actual build, not in Unity Editor");
        Application.Quit();
    }
}

