
using UnityEngine;

public class SaveCheckpoint : MonoBehaviour
{
    [Tooltip("Optional. Leave empty to use the entering Player.")]
    public GameObject player;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Use the Player that entered this checkpoint.
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

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (player == null)
        {
            Debug.LogError("[SAVE] Player not found.");
            return;
        }

        GameSaveManager.Instance.SaveCheckpoint(player);

        Debug.Log("[CHECKPOINT] Saved position: " + player.transform.position);
    }
}
