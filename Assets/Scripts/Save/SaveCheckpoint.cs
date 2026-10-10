using UnityEngine;

public class SaveCheckpoint : MonoBehaviour
{
    public GameObject player;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Save();
    }

    public void Save()
    {
        if (GameSaveManager.Instance == null)
            return;

        GameSaveManager.Instance.SaveCheckpoint(
            player
        );

        GameSaveManager.Instance.SaveToDisk();

        Debug.Log(
            "========== CHECKPOINT =========="
        );

        GameSaveManager.Instance.DebugSave();
    }
}
