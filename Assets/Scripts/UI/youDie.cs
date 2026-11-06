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

    public void RestartFromCheckpoint()
    {
        if (GameSaveManager.Instance != null)
        {
            GameSaveManager.Instance.ReloadCheckpoint();
        }
        else
        {
            Debug.LogError("GameSaveManager not found!");
        }
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
                GameSaveManager.Instance.SavePlayerProgress(
                    playerScript.gameObject
                );

                Debug.Log("Player progress saved before returning to menu.");
            }
            else
            {
                Debug.LogError("Player Script is not assigned in youDie!");
            }

            GameSaveManager.Instance.DebugSave();
        }
        else
        {
            Debug.LogError("GameSaveManager not found!");
        }

        Time.timeScale = 1f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene(MainMenuSceneName);
    }
}
