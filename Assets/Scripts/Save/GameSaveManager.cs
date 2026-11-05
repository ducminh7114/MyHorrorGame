
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

    // Backward compatibility with existing scripts.
    public void SaveCheckpoint(GameObject player)
    {
        if (player == null)
        {
            Debug.LogError("[SAVE] Cannot save: Player is missing.");
            return;
        }

        SaveCheckpoint(
            player,
            player.transform.position,
            player.transform.rotation
        );
    }

    // Save Player data and checkpoint Transform separately.
    public void SaveCheckpoint(
        GameObject player,
        Vector3 checkpointPosition,
        Quaternion checkpointRotation)
    {
        if (player == null)
        {
            Debug.LogError("[SAVE] Cannot save: Player is missing.");
            return;
        }

        currentSave.hasSave = true;
        currentSave.sceneName = SceneManager.GetActiveScene().name;

        // Save Player's current position.
        Vector3 playerPos = player.transform.position;
        Vector3 playerRot = player.transform.eulerAngles;

        currentSave.playerPosX = playerPos.x;
        currentSave.playerPosY = playerPos.y;
        currentSave.playerPosZ = playerPos.z;

        currentSave.playerRotX = playerRot.x;
        currentSave.playerRotY = playerRot.y;
        currentSave.playerRotZ = playerRot.z;

        // Save checkpoint position and rotation.
        currentSave.checkpointPosition = checkpointPosition;
        currentSave.checkpointRotation = checkpointRotation;
        currentSave.hasCheckpointPosition = true;

        Debug.Log(
            "[SAVE] Player position: " + playerPos +
            " | Checkpoint position: " + checkpointPosition
        );

        SaveToDisk();
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

            SaveToDisk();
        }
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

    // ---------- SAVE ITEM STATE WITHOUT CHECKPOINT ----------

    public void SaveItemState(string id, bool active)
    {
        if (string.IsNullOrWhiteSpace(id))
            return;

        SetObjectState(id, active);

        // Save JSON immediately.
        // IMPORTANT: Do not modify hasSave.
        SaveToDisk();

        Debug.Log("[SAVE] Independent item state saved: " + id);
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

            if (currentSave.weaponInventory == null)
                currentSave.weaponInventory = new WeaponInventorySaveData();

            if (currentSave.weaponInventory.ammo == null)
                currentSave.weaponInventory.ammo = new List<WeaponAmmoSaveData>();

            if (currentSave.collectedWeaponPickupIDs == null)
                currentSave.collectedWeaponPickupIDs = new List<string>();

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

        // Allow scene objects and Start methods to initialize.
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
            FindObjectsOfType<PersistentObject>(true);

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

            // Determine restore position.
            Vector3 restorePosition;
            Quaternion restoreRotation;

            if (currentSave.hasCheckpointPosition)
            {
                restorePosition =
                    currentSave.checkpointPosition;

                restoreRotation =
                    currentSave.checkpointRotation;

                Debug.Log(
                    "[LOAD] Restoring to checkpoint: " +
                    restorePosition
                );
            }
            else
            {
                // Backward compatibility with older save files.
                restorePosition = new Vector3(
                    currentSave.playerPosX,
                    currentSave.playerPosY,
                    currentSave.playerPosZ
                );

                restoreRotation = Quaternion.Euler(
                    currentSave.playerRotX,
                    currentSave.playerRotY,
                    currentSave.playerRotZ
                );

                Debug.Log(
                    "[LOAD] Restoring to previous Player position."
                );
            }

            // Restore Player.
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
            Debug.LogWarning(
                "[SAVE] Player with tag Player not found."
            );
        }

        Debug.Log("[SAVE] Restore finished.");

        restoring = false;
    }

    // ---------- DEBUG ----------

    public void DebugSave()
    {
        Debug.Log("[SAVE] Save file path: " + SavePath);
        Debug.Log("[SAVE] Has save: " + HasSaveGame());
        Debug.Log("[SAVE] Saved scene: " + GetSavedScene());

        Debug.Log(
            "[SAVE] Completed triggers: " +
            currentSave.completedTriggers.Count
        );

        Debug.Log(
            "[SAVE] Saved object states: " +
            currentSave.objectStates.Count
        );

        if (currentSave.hasCheckpointPosition)
        {
            Debug.Log(
                "[SAVE] Checkpoint position: " +
                currentSave.checkpointPosition
            );
        }

        SaveToDisk();
    }

    // ---------- WEAPON INVENTORY ----------

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
            currentSave.weaponInventory.ammo = new List<WeaponAmmoSaveData>();

        return currentSave.weaponInventory;
    }

    // ---------- WEAPON PICKUPS ----------

    public void MarkWeaponPickupCollected(string pickupID)
    {
        if (string.IsNullOrEmpty(pickupID))
            return;

        if (currentSave == null)
            currentSave = new SaveData();

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
        return currentSave != null
            && currentSave.collectedWeaponPickupIDs != null
            && currentSave.collectedWeaponPickupIDs.Contains(pickupID);
    }

    // ---------- TRIGGER CHECK ----------

    public bool IsTriggerCompleted(string triggerID)
    {
        return !string.IsNullOrWhiteSpace(triggerID)
            && currentSave != null
            && currentSave.completedTriggers != null
            && currentSave.completedTriggers.Contains(triggerID);
    }
}
