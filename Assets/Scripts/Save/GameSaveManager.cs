
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

    // Progress since the last checkpoint.
    private readonly HashSet<string> pendingTriggerIDs =
        new HashSet<string>();

    // Object changes since the last checkpoint.
    private readonly Dictionary<string, bool> pendingObjectStates =
        new Dictionary<string, bool>();

    private bool restoring;

    // =========================================================
    // INITIALIZATION
    // =========================================================

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

    // =========================================================
    // NEW GAME
    // =========================================================

    public void NewGame()
    {
        currentSave = new SaveData();

        pendingTriggerIDs.Clear();
        pendingObjectStates.Clear();

        if (File.Exists(SavePath))
            File.Delete(SavePath);

        PlayerPrefs.SetInt("HasSaveGame", 0);
        PlayerPrefs.Save();

        Debug.Log("[SAVE] New game started. Old save deleted.");
    }

    // =========================================================
    // CHECKPOINT
    // =========================================================

    // Compatibility overload:
    // Saves the player's current position as the checkpoint.
    public void SaveCheckpoint(GameObject player)
    {
        if (player == null)
        {
            Debug.LogError("[SAVE] Cannot save checkpoint: Player is NULL.");
            return;
        }

        SaveCheckpoint(
            player,
            player.transform.position,
            player.transform.rotation
        );
    }

    // Saves an explicitly supplied checkpoint position.
    public void SaveCheckpoint(
        GameObject player,
        Vector3 checkpointPosition,
        Quaternion checkpointRotation)
    {
        if (player == null)
        {
            Debug.LogError("[SAVE] Cannot save checkpoint: Player is NULL.");
            return;
        }

        if (currentSave == null)
            currentSave = new SaveData();

        // Commit all temporary progress.
        CommitPendingProgress();

        currentSave.hasSave = true;
        currentSave.sceneName = SceneManager.GetActiveScene().name;

        // Checkpoint respawn position.
        currentSave.checkpointPosition = checkpointPosition;
        currentSave.checkpointRotation = checkpointRotation;
        currentSave.hasCheckpointPosition = true;

        // Keep current player position as backup.
        Vector3 playerPos = player.transform.position;
        Vector3 playerRot = player.transform.eulerAngles;

        currentSave.playerPosX = playerPos.x;
        currentSave.playerPosY = playerPos.y;
        currentSave.playerPosZ = playerPos.z;

        currentSave.playerRotX = playerRot.x;
        currentSave.playerRotY = playerRot.y;
        currentSave.playerRotZ = playerRot.z;

        SaveToDisk();

        Debug.Log("====================================");
        Debug.Log("[SAVE] CHECKPOINT SAVED");
        Debug.Log("[SAVE] Scene: " + currentSave.sceneName);
        Debug.Log("[SAVE] Respawn position: " + checkpointPosition);
        Debug.Log("[SAVE] Committed trigger count: " +
                  currentSave.completedTriggers.Count);
        Debug.Log("====================================");
    }

    // =========================================================
    // COMMIT TEMPORARY PROGRESS
    // =========================================================

    private void CommitPendingProgress()
    {
        // Commit trigger IDs.
        foreach (string id in pendingTriggerIDs)
        {
            if (!currentSave.completedTriggers.Contains(id))
            {
                currentSave.completedTriggers.Add(id);

                Debug.Log("[SAVE] CHECKPOINT COMMITTED TRIGGER: " + id);
            }
        }

        pendingTriggerIDs.Clear();

        // Commit object states.
        foreach (KeyValuePair<string, bool> entry in pendingObjectStates)
        {
            ObjectStateData state =
                currentSave.objectStates.Find(x => x.id == entry.Key);

            if (state == null)
            {
                state = new ObjectStateData();
                state.id = entry.Key;
                currentSave.objectStates.Add(state);
            }

            state.active = entry.Value;

            Debug.Log(
                "[SAVE] CHECKPOINT COMMITTED OBJECT: " +
                entry.Key + " = " + entry.Value
            );
        }

        pendingObjectStates.Clear();
    }

    // =========================================================
    // TRIGGER PROGRESS
    // =========================================================

    public void MarkTriggerCompleted(string triggerID)
    {
        if (string.IsNullOrWhiteSpace(triggerID))
        {
            Debug.LogWarning("[SAVE] Trigger ID is empty.");
            return;
        }

        if (IsTriggerCompleted(triggerID))
            return;

        // IMPORTANT:
        // Do NOT add this to currentSave.completedTriggers.
        // Do NOT write it to JSON here.

        pendingTriggerIDs.Add(triggerID);

        Debug.Log(
            "[SAVE] TEMPORARY TRIGGER COMPLETED: " +
            triggerID +
            " | Pending count: " +
            pendingTriggerIDs.Count
        );
    }

    public bool IsTriggerCompleted(string triggerID)
    {
        if (string.IsNullOrWhiteSpace(triggerID) ||
            currentSave == null)
            return false;

        bool permanentlySaved =
            currentSave.completedTriggers != null &&
            currentSave.completedTriggers.Contains(triggerID);

        bool completedThisSession =
            pendingTriggerIDs.Contains(triggerID);

        return permanentlySaved || completedThisSession;
    }

    // =========================================================
    // DISCARD TEMPORARY PROGRESS
    // =========================================================

    public void DiscardPendingProgress()
    {
        Debug.Log("[SAVE] Discarding uncommitted progress.");

        foreach (string id in pendingTriggerIDs)
            Debug.Log("[SAVE] Trigger will be available again: " + id);

        pendingTriggerIDs.Clear();
        pendingObjectStates.Clear();
    }

    // =========================================================
    // RELOAD LAST CHECKPOINT
    // =========================================================

    public void ReloadCheckpoint()
    {
        if (!HasSaveGame())
        {
            Debug.LogError("[SAVE] Cannot reload: No checkpoint exists.");
            return;
        }

        string savedScene = currentSave.sceneName;

        // Discard temporary progress before reloading.
        DiscardPendingProgress();

        // Reload the actual saved data from JSON.
        // This also discards unsaved in-memory changes.
        LoadFromDisk();

        if (!HasSaveGame())
        {
            Debug.LogError("[SAVE] Checkpoint data could not be loaded.");
            return;
        }

        savedScene = currentSave.sceneName;

        Debug.Log("[SAVE] Reloading checkpoint scene: " + savedScene);

        SceneManager.LoadScene(savedScene);
    }

    // =========================================================
    // OBJECT STATES
    // =========================================================

    // Temporary object state. It is committed at a checkpoint.
    public void SetObjectState(string id, bool active)
    {
        if (string.IsNullOrWhiteSpace(id))
            return;

        pendingObjectStates[id] = active;

        Debug.Log(
            "[SAVE] TEMPORARY OBJECT STATE: " +
            id + " = " + active
        );
    }

    public bool TryGetObjectState(string id, out bool active)
    {
        active = true;

        if (string.IsNullOrWhiteSpace(id) || currentSave == null)
            return false;

        // Temporary state takes priority during this session.
        if (pendingObjectStates.TryGetValue(id, out active))
            return true;

        if (currentSave.objectStates == null)
            return false;

        ObjectStateData state =
            currentSave.objectStates.Find(x => x.id == id);

        if (state == null)
            return false;

        active = state.active;
        return true;
    }

    // =========================================================
    // INDEPENDENT ITEM STATE SAVE
    // =========================================================

    // Use this when an item should persist independently
    // of the next checkpoint.
    public void SaveItemState(string id, bool active)
    {
        if (string.IsNullOrWhiteSpace(id))
            return;

        ObjectStateData state =
            currentSave.objectStates.Find(x => x.id == id);

        if (state == null)
        {
            state = new ObjectStateData();
            state.id = id;
            currentSave.objectStates.Add(state);
        }

        state.active = active;

        // This ID is now permanently saved, not pending.
        pendingObjectStates.Remove(id);

        SaveToDisk();

        Debug.Log(
            "[SAVE] Independent item state saved: " +
            id + " = " + active
        );
    }

    // =========================================================
    // SAVE PLAYER PROGRESS WITHOUT MOVING CHECKPOINT
    // =========================================================

    public void SavePlayerProgress(GameObject player)
    {
        if (player == null)
        {
            Debug.LogError("[SAVE] Player is missing.");
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

        // This does NOT commit pending triggers or objects.
        // It does NOT change the checkpoint transform.

        SaveToDisk();

        Debug.Log("[SAVE] Player progress saved: " + pos);
    }

    // =========================================================
    // WEAPON INVENTORY
    // =========================================================

    public void SaveWeaponInventory(WeaponInventorySaveData data)
    {
        if (currentSave == null)
            currentSave = new SaveData();

        currentSave.weaponInventory = data;

        if (currentSave.hasSave)
            SaveToDisk();
    }

    public WeaponInventorySaveData GetWeaponInventory()
    {
        if (currentSave == null)
            currentSave = new SaveData();

        if (currentSave.weaponInventory == null)
            currentSave.weaponInventory = new WeaponInventorySaveData();

        if (currentSave.weaponInventory.ammo == null)
            currentSave.weaponInventory.ammo =
                new List<WeaponAmmoSaveData>();

        return currentSave.weaponInventory;
    }

    // =========================================================
    // WEAPON PICKUPS
    // =========================================================

    public void MarkWeaponPickupCollected(string pickupID)
    {
        if (string.IsNullOrWhiteSpace(pickupID))
            return;

        if (currentSave.collectedWeaponPickupIDs == null)
            currentSave.collectedWeaponPickupIDs = new List<string>();

        if (!currentSave.collectedWeaponPickupIDs.Contains(pickupID))
        {
            currentSave.collectedWeaponPickupIDs.Add(pickupID);

            if (currentSave.hasSave)
                SaveToDisk();
        }
    }

    public bool WasWeaponPickupCollected(string pickupID)
    {
        return currentSave != null &&
               currentSave.collectedWeaponPickupIDs != null &&
               currentSave.collectedWeaponPickupIDs.Contains(pickupID);
    }

    // =========================================================
    // SAVE FILE
    // =========================================================

    public void SaveToDisk()
    {
        if (currentSave == null)
            currentSave = new SaveData();

        try
        {
            string json = JsonUtility.ToJson(currentSave, true);

            File.WriteAllText(SavePath, json);

            PlayerPrefs.SetInt("HasSaveGame", currentSave.hasSave ? 1 : 0);
            PlayerPrefs.Save();

            Debug.Log("[SAVE] File written: " + SavePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("[SAVE] Could not write save: " + e.Message);
        }
    }

    public void LoadFromDisk()
    {
        pendingTriggerIDs.Clear();
        pendingObjectStates.Clear();

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

            if (currentSave.weaponInventory == null)
                currentSave.weaponInventory = new WeaponInventorySaveData();

            if (currentSave.weaponInventory.ammo == null)
                currentSave.weaponInventory.ammo =
                    new List<WeaponAmmoSaveData>();

            if (currentSave.collectedWeaponPickupIDs == null)
                currentSave.collectedWeaponPickupIDs = new List<string>();

            Debug.Log("[SAVE] Save file loaded.");
            Debug.Log("[SAVE] Permanently completed triggers: " +
                      currentSave.completedTriggers.Count);
        }
        catch (System.Exception e)
        {
            Debug.LogError("[SAVE] Could not load save: " + e.Message);
            currentSave = new SaveData();
        }
    }

    // =========================================================
    // CONTINUE INFORMATION
    // =========================================================

    public bool HasSaveGame()
    {
        return currentSave != null &&
               currentSave.hasSave &&
               !string.IsNullOrEmpty(currentSave.sceneName);
    }

    public string GetSavedScene()
    {
        return currentSave != null ? currentSave.sceneName : "";
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
    // RESTORE AFTER CONTINUE / CHECKPOINT RELOAD
    // =========================================================

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!HasSaveGame() || scene.name != currentSave.sceneName)
            return;

        // Reload the committed snapshot before trigger Start methods run.
        LoadFromDisk();

        Debug.Log("[SAVE] Checkpoint scene loaded: " + scene.name);

        StartCoroutine(RestoreSceneData(scene.name));
    }

    private IEnumerator RestoreSceneData(string sceneName)
    {
        if (restoring)
            yield break;

        restoring = true;

        // Allow scene objects to initialize.
        yield return null;
        yield return null;

        if (!HasSaveGame() || sceneName != currentSave.sceneName)
        {
            restoring = false;
            yield break;
        }

        Debug.Log("[SAVE] Restoring scene: " + sceneName);

        // Restore persistent objects.
        PersistentObject[] objects =
            FindObjectsByType<PersistentObject>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None
            );

        foreach (PersistentObject obj in objects)
        {
            if (obj != null)
                obj.RestoreState();
        }

        // Find Player.
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            CharacterController controller =
                player.GetComponent<CharacterController>();

            if (controller != null)
                controller.enabled = false;

            Vector3 restorePosition;
            Quaternion restoreRotation;

            if (currentSave.hasCheckpointPosition)
            {
                restorePosition = currentSave.checkpointPosition;
                restoreRotation = currentSave.checkpointRotation;

                Debug.Log(
                    "[LOAD] Restoring checkpoint: " + restorePosition
                );
            }
            else
            {
                // Compatibility with older save files.
                restorePosition = GetSavedPosition();
                restoreRotation = Quaternion.Euler(GetSavedRotation());

                Debug.Log(
                    "[LOAD] Restoring previous player position."
                );
            }

            player.transform.SetPositionAndRotation(
                restorePosition,
                restoreRotation
            );

            if (controller != null)
                controller.enabled = true;

            playerMovement movement =
                player.GetComponent<playerMovement>();

            if (movement != null)
                movement.enabled = true;

            Debug.Log(
                "[LOAD] Player restored at: " +
                player.transform.position
            );
        }
        else
        {
            Debug.LogWarning("[SAVE] Player with tag Player not found.");
        }

        Debug.Log("[SAVE] Restore finished.");

        restoring = false;
    }

    // =========================================================
    // DEBUG
    // =========================================================

    public void DebugSave()
    {
        Debug.Log("========== SAVE DEBUG ==========");

        Debug.Log("[SAVE] Has save: " + HasSaveGame());
        Debug.Log("[SAVE] Scene: " + GetSavedScene());
        Debug.Log("[SAVE] Checkpoint: " +
                  currentSave.checkpointPosition);

        Debug.Log("----- PERMANENT TRIGGERS -----");

        foreach (string id in currentSave.completedTriggers)
            Debug.Log("[PERMANENT] " + id);

        Debug.Log("----- TEMPORARY TRIGGERS -----");

        foreach (string id in pendingTriggerIDs)
            Debug.Log("[PENDING] " + id);

        Debug.Log("----- TEMPORARY OBJECT STATES -----");

        foreach (KeyValuePair<string, bool> entry in pendingObjectStates)
        {
            Debug.Log(
                "[PENDING OBJECT] " +
                entry.Key + " = " + entry.Value
            );
        }

        Debug.Log("================================");
    }

    [ContextMenu("DEBUG - List Trigger Progress")]
    public void DebugListTriggerProgress()
    {
        DebugSave();
    }
}
