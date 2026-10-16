
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSaveManager : MonoBehaviour
{
    public static GameSaveManager Instance;

    public SaveData currentSave = new SaveData();

    private string SavePath =>
        Path.Combine(Application.persistentDataPath, "savegame.json");

    private bool restoring;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadFromDisk();

        SceneManager.sceneLoaded += OnSceneLoaded;

        Debug.Log("[SAVE] Manager ready: " + SavePath);
    }

    private void OnDestroy()
    {
        if (Instance != this)
            return;

        SceneManager.sceneLoaded -= OnSceneLoaded;
        Instance = null;
    }

    // ---------- NEW GAME ----------

    public void NewGame()
    {
        currentSave = new SaveData();

        if (File.Exists(SavePath))
            File.Delete(SavePath);

        Debug.Log("[SAVE] New game started. Old save deleted.");
    }

    // ---------- CHECKPOINT ----------

    public void SaveCheckpoint(GameObject player)
    {
        if (player == null)
        {
            Debug.LogError("[SAVE] Cannot save: Player is missing.");
            return;
        }

        currentSave.hasSave = true;
        currentSave.sceneName = SceneManager.GetActiveScene().name;

        Vector3 pos = player.transform.position;
        Vector3 rot = player.transform.eulerAngles;

        currentSave.playerPosX = pos.x;
        currentSave.playerPosY = pos.y;
        currentSave.playerPosZ = pos.z;

        currentSave.playerRotX = rot.x;
        currentSave.playerRotY = rot.y;
        currentSave.playerRotZ = rot.z;

        SaveToDisk();

        Debug.Log("[SAVE] Checkpoint saved in scene: " +
                  currentSave.sceneName);
    }

    // ---------- COMPLETED EVENTS ----------

    public void MarkTriggerCompleted(string triggerID)
    {
        if (string.IsNullOrWhiteSpace(triggerID))
        {
            Debug.LogWarning("[SAVE] Trigger ID is empty.");
            return;
        }

        if (!currentSave.completedTriggers.Contains(triggerID))
        {
            currentSave.completedTriggers.Add(triggerID);
            Debug.Log("[SAVE] Event recorded: " + triggerID);
        }
    }

    public bool IsTriggerCompleted(string triggerID)
    {
        return !string.IsNullOrWhiteSpace(triggerID)
            && currentSave.completedTriggers.Contains(triggerID);
    }

    // ---------- OBJECT STATES ----------

    public void SetObjectState(string id, bool active)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            Debug.LogWarning("[SAVE] Object ID is empty.");
            return;
        }

        ObjectStateData state =
            currentSave.objectStates.Find(item => item.id == id);

        if (state == null)
        {
            state = new ObjectStateData();
            state.id = id;
            currentSave.objectStates.Add(state);
        }

        state.active = active;

        Debug.Log("[SAVE] Object recorded: " + id + " = " + active);
    }

    public bool TryGetObjectState(string id, out bool active)
    {
        ObjectStateData state =
            currentSave.objectStates.Find(item => item.id == id);

        if (state != null)
        {
            active = state.active;
            return true;
        }

        active = false;
        return false;
    }

    // ---------- DISK SAVE / LOAD ----------

    public void SaveToDisk()
    {
        try
        {
            string json = JsonUtility.ToJson(currentSave, true);
            File.WriteAllText(SavePath, json);

            Debug.Log("[SAVE] File written successfully.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("[SAVE] Could not write save: " + e.Message);
        }
    }

    public void LoadFromDisk()
    {
        if (!File.Exists(SavePath))
        {
            currentSave = new SaveData();
            Debug.Log("[SAVE] No existing save file.");
            return;
        }

        try
        {
            string json = File.ReadAllText(SavePath);
            currentSave = JsonUtility.FromJson<SaveData>(json);

            if (currentSave == null)
                currentSave = new SaveData();

            if (currentSave.completedTriggers == null)
                currentSave.completedTriggers = new List<string>();

            if (currentSave.objectStates == null)
                currentSave.objectStates = new List<ObjectStateData>();

            Debug.Log("[SAVE] Save file loaded.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("[SAVE] Could not load save: " + e.Message);
            currentSave = new SaveData();
        }
    }

    public bool HasSaveGame()
    {
        return currentSave != null
            && currentSave.hasSave
            && !string.IsNullOrEmpty(currentSave.sceneName);
    }

    public string GetSavedScene()
    {
        return currentSave.sceneName;
    }

    // ---------- RESTORE AFTER CONTINUE ----------

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (HasSaveGame() && scene.name == currentSave.sceneName)
            StartCoroutine(RestoreSceneData(scene.name));
    }

    private IEnumerator RestoreSceneData(string sceneName)
    {
        if (restoring)
            yield break;

        restoring = true;

        // Allow scene objects and their Start methods to initialize.
        yield return null;
        yield return null;

        if (!HasSaveGame() || sceneName != currentSave.sceneName)
        {
            restoring = false;
            yield break;
        }

        Debug.Log("[SAVE] Restoring scene: " + sceneName);

        PersistentObject[] objects =
            FindObjectsOfType<PersistentObject>(true);

        foreach (PersistentObject obj in objects)
        {
            if (obj != null)
                obj.RestoreState();
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            CharacterController controller =
                player.GetComponent<CharacterController>();

            if (controller != null)
                controller.enabled = false;

            player.transform.SetPositionAndRotation(
                new Vector3(
                    currentSave.playerPosX,
                    currentSave.playerPosY,
                    currentSave.playerPosZ
                ),
                Quaternion.Euler(
                    currentSave.playerRotX,
                    currentSave.playerRotY,
                    currentSave.playerRotZ
                )
            );

            if (controller != null)
                controller.enabled = true;

            Debug.Log("[SAVE] Player position restored.");
        }
        else
        {
            Debug.LogWarning("[SAVE] Player with tag Player not found.");
        }

        Debug.Log("[SAVE] Restore finished.");
        restoring = false;
    }

    public void DebugSave()
    {
        Debug.Log("[SAVE] Save file path: " + SavePath);
        Debug.Log("[SAVE] Has save: " + HasSaveGame());
        Debug.Log("[SAVE] Saved scene: " + GetSavedScene());
        Debug.Log("[SAVE] Completed triggers: " +
            currentSave.completedTriggers.Count);
        Debug.Log("[SAVE] Saved object states: " +
            currentSave.objectStates.Count);

        SaveToDisk();
    }
}