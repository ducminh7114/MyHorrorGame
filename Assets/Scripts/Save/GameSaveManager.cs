using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSaveManager : MonoBehaviour
{
    public static GameSaveManager Instance;

    private SaveData currentSave;

    private string savePath;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        savePath = Path.Combine(
            Application.persistentDataPath,
            "savegame.json"
        );

        LoadSaveFromDisk();

        Debug.Log("GameSaveManager initialized.");
        Debug.Log("Save path: " + savePath);
    }

    // =========================================================
    // LOAD SAVE FILE
    // =========================================================

    private void LoadSaveFromDisk()
    {
        if (!File.Exists(savePath))
        {
            currentSave = new SaveData();

            Debug.Log("No save file found.");
            return;
        }

        string json = File.ReadAllText(savePath);

        currentSave = JsonUtility.FromJson<SaveData>(json);

        if (currentSave == null)
        {
            currentSave = new SaveData();
        }

        Debug.Log("Save file loaded.");
    }

    // =========================================================
    // NEW GAME
    // =========================================================

    public void NewGame()
    {
        currentSave = new SaveData();

        DeleteSaveFile();

        Debug.Log("NEW GAME - save data cleared.");
    }

    // =========================================================
    // CHECK SAVE
    // =========================================================

    public bool HasSaveGame()
    {
        return currentSave != null &&
               currentSave.hasSave;
    }

    // =========================================================
    // SAVE PLAYER + SCENE
    // =========================================================

    public void SaveCheckpoint(GameObject player)
    {
        if (player == null)
        {
            Debug.LogError("SaveCheckpoint: Player is NULL.");
            return;
        }

        currentSave.hasSave = true;

        currentSave.sceneName =
            SceneManager.GetActiveScene().name;

        currentSave.playerPosX =
            player.transform.position.x;

        currentSave.playerPosY =
            player.transform.position.y;

        currentSave.playerPosZ =
            player.transform.position.z;

        currentSave.playerRotX =
            player.transform.eulerAngles.x;

        currentSave.playerRotY =
            player.transform.eulerAngles.y;

        currentSave.playerRotZ =
            player.transform.eulerAngles.z;

        SaveToDisk();

        Debug.Log("=================================");
        Debug.Log("CHECKPOINT SAVED");
        Debug.Log("Scene: " + currentSave.sceneName);
        Debug.Log("Position: " + player.transform.position);
        Debug.Log("=================================");
    }

    // =========================================================
    // TRIGGERS
    // =========================================================

    public void MarkTriggerCompleted(string triggerID)
    {
        if (string.IsNullOrEmpty(triggerID))
            return;

        if (currentSave.completedTriggers == null)
        {
            currentSave.completedTriggers =
                new List<string>();
        }

        if (!currentSave.completedTriggers.Contains(triggerID))
        {
            currentSave.completedTriggers.Add(triggerID);

            Debug.Log(
                "Trigger completed: " +
                triggerID
            );
        }
    }

    public bool IsTriggerCompleted(string triggerID)
    {
        if (string.IsNullOrEmpty(triggerID))
            return false;

        if (currentSave.completedTriggers == null)
            return false;

        return currentSave.completedTriggers.Contains(triggerID);
    }

    // =========================================================
    // OBJECT STATE
    // =========================================================

    public void SetObjectState(
        string id,
        bool active
    )
    {
        if (string.IsNullOrEmpty(id))
            return;

        ObjectStateData state =
            currentSave.objectStates.Find(
                x => x.id == id
            );

        if (state == null)
        {
            state = new ObjectStateData();

            state.id = id;
            state.active = active;

            currentSave.objectStates.Add(state);
        }
        else
        {
            state.active = active;
        }

        Debug.Log(
            "Saved object state: " +
            id +
            " = " +
            active
        );
    }

    public bool TryGetObjectState(
        string id,
        out bool active
    )
    {
        active = true;

        ObjectStateData state =
            currentSave.objectStates.Find(
                x => x.id == id
            );

        if (state == null)
            return false;

        active = state.active;

        return true;
    }

    // =========================================================
    // ACTUALLY WRITE FILE
    // =========================================================

    public void SaveToDisk()
    {
        if (currentSave == null)
            currentSave = new SaveData();

        string json =
            JsonUtility.ToJson(
                currentSave,
                true
            );

        File.WriteAllText(
            savePath,
            json
        );

        Debug.Log("SAVE FILE WRITTEN.");
    }

    // =========================================================
    // DELETE SAVE
    // =========================================================

    public void DeleteSaveFile()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
        }

        currentSave = new SaveData();

        Debug.Log("SAVE FILE DELETED.");
    }

    // =========================================================
    // CONTINUE INFORMATION
    // =========================================================

    public string GetSavedScene()
    {
        if (currentSave == null)
            return "";

        return currentSave.sceneName;
    }

    public Vector3 GetSavedPosition()
    {
        return new Vector3(
            currentSave.playerPosX,
            currentSave.playerPosY,
            currentSave.playerPosZ
        );
    }

    public Vector3 GetSavedRotation()
    {
        return new Vector3(
            currentSave.playerRotX,
            currentSave.playerRotY,
            currentSave.playerRotZ
        );
    }

    // =========================================================
    // DEBUG
    // =========================================================

    public void DebugSave()
    {
        Debug.Log("========== SAVE DATA ==========");

        Debug.Log(
            "Has Save: " +
            currentSave.hasSave
        );

        Debug.Log(
            "Scene: " +
            currentSave.sceneName
        );

        Debug.Log(
            "Player Position: " +
            GetSavedPosition()
        );

        Debug.Log(
            "Completed Triggers:"
        );

        foreach (
            string trigger
            in currentSave.completedTriggers
        )
        {
            Debug.Log(" - " + trigger);
        }

        Debug.Log(
            "Object States:"
        );

        foreach (
            ObjectStateData state
            in currentSave.objectStates
        )
        {
            Debug.Log(
                " - " +
                state.id +
                " = " +
                state.active
            );
        }

        Debug.Log("===============================");
    }
}