using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class youDie : MonoBehaviour
{
    [Header("UI")]
    public Button restartButton;

    [Header("Scenes")]
    public string SceneName;
    public string MainMenuSceneName;
    public playerMovement playerScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        UpdateContinueButton();

    }

    // Update is called once per frame
    void Update()
    {
        UpdateContinueButton();
    }

    void UpdateContinueButton()
    {
        if(restartButton == null)
        {
            return;
        }
        restartButton.interactable = GameSaveManager.Instance != null && GameSaveManager.Instance.HasSaveGame();
    }

    public void ContinueGame()
    {
        if(GameSaveManager.Instance == null)
        {
            Debug.LogError("GameSaveManager instance is null. Cannot continue game.");
            return;
        }
        if (!GameSaveManager.Instance.HasSaveGame())
        {
            Debug.LogError("No save game found. Cannot continue game.");
            return;
        }
        string scene = GameSaveManager.Instance.GetSavedScene();

        Debug.Log("Loading scene: " + scene);

        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
    }


    public void QuitToMainMenu()
    {
        Debug.Log("=================================");
        Debug.Log("QUITTING TO MAIN MENU");
        Debug.Log("=================================");

        if (GameSaveManager.Instance != null)
        {
            if (playerScript != null)
            {
                GameSaveManager.Instance.SaveCheckpoint(
                    playerScript.gameObject
                );

                Debug.Log("Player progress saved before returning to menu.");
            }
            else
            {
                Debug.LogError(
                    "Player Script is not assigned in PauseGame!"
                );
            }

            GameSaveManager.Instance.DebugSave();
        }
        else
        {
            Debug.LogError("GameSaveManager not found!");
        }

        // Restore normal time before loading the menu.
        Time.timeScale = 1f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene("MainMenu");
    }
}
