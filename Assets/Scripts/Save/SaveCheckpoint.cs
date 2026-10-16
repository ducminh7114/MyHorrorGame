
using UnityEngine;

public class SaveCheckpoint : MonoBehaviour
{
    [Tooltip("Optional. If empty, the entering Player is used.")]
    public GameObject player;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (player == null)
            player = other.gameObject;

        Save();
    }

    public void Save()
    {
        if (GameSaveManager.Instance == null)
        {
            Debug.LogError("[SAVE] GameSaveManager not found.");
            return;
        }

        GameObject targetPlayer = player;

        if (targetPlayer == null)
            targetPlayer = GameObject.FindGameObjectWithTag("Player");

        GameSaveManager.Instance.SaveCheckpoint(targetPlayer);
    }
}