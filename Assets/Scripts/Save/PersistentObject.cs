
using UnityEngine;

public class PersistentObject : MonoBehaviour
{
    [Header("Must be unique in the entire game")]
    public string objectID;

    public void RestoreState()
    {
        if (string.IsNullOrWhiteSpace(objectID))
        {
            Debug.LogWarning(
                "[SAVE] Missing Object ID on " + gameObject.name
            );
            return;
        }

        if (GameSaveManager.Instance == null)
            return;

        if (GameSaveManager.Instance.TryGetObjectState(
            objectID, out bool savedActive))
        {
            gameObject.SetActive(savedActive);

            Debug.Log(
                "[SAVE] Restored " + objectID + " = " + savedActive
            );
        }
    }
}