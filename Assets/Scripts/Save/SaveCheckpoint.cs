
using UnityEngine;

public class SaveCheckpoint : MonoBehaviour
{
    [Header("Checkpoint Position")]
    [SerializeField] private bool useCustomPosition = false;
    [SerializeField] private Vector3 customPosition;
    [SerializeField] private Vector3 customRotation;

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

        // Choose checkpoint position
        Vector3 checkpointPosition = useCustomPosition
            ? customPosition
            : transform.position;

        Quaternion checkpointRotation = useCustomPosition
            ? Quaternion.Euler(customRotation)
            : transform.rotation;

        GameSaveManager.Instance.SaveCheckpoint(
            player,
            checkpointPosition,
            checkpointRotation
        );

        Debug.Log(
            "[CHECKPOINT] " + gameObject.name +
            " saved at: " + checkpointPosition +
            " rotation: " + checkpointRotation.eulerAngles
        );
    }

    private void OnDrawGizmos()
    {
        Vector3 position = useCustomPosition
            ? customPosition
            : transform.position;

        Quaternion rotation = useCustomPosition
            ? Quaternion.Euler(customRotation)
            : transform.rotation;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(position, 0.3f);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(position, rotation * Vector3.forward * 1.5f);
    }

    [ContextMenu("Set Custom Position To Transform")]
    private void SetCustomPositionToTransform()
    {
        customPosition = transform.position;
        customRotation = transform.eulerAngles;
    }
}

