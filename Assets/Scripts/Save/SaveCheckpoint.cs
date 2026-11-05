using UnityEngine;

public class SaveCheckpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Save(other.gameObject);
    }

    public void Save(GameObject player)
    {
        if (GameSaveManager.Instance == null)
        {
            Debug.LogError("[SAVE] GameSaveManager not found.");
            return;
        }

        if (player == null)
        {
            Debug.LogError("[SAVE] Player not found.");
            return;
        }

        // Get this checkpoint object's Transform.
        Vector3 checkpointPosition = transform.position;
        Quaternion checkpointRotation = transform.rotation;

        GameSaveManager.Instance.SaveCheckpoint(
            player,
            checkpointPosition,
            checkpointRotation
        );

        Debug.Log(
            "[CHECKPOINT] " + gameObject.name +
            " saved at: " + checkpointPosition
        );
    }
}
