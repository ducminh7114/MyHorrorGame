using UnityEngine;

public class PersistentObject : MonoBehaviour
{
    [Header("Unique Save ID")]
    public string objectID;

    private void Start()
    {
        if (string.IsNullOrEmpty(objectID))
        {
            Debug.LogWarning(
                "PersistentObject has no ID: " +
                gameObject.name
            );

            return;
        }

        RestoreState();
    }

    public void SetState(bool active)
    {
        if (GameSaveManager.Instance == null)
            return;

        gameObject.SetActive(active);

        GameSaveManager.Instance.SetObjectState(
            objectID,
            active
        );
    }

    public void RestoreState()
    {
        if (GameSaveManager.Instance == null)
            return;

        bool savedState;

        bool found =
            GameSaveManager.Instance.TryGetObjectState(
                objectID,
                out savedState
            );

        if (!found)
            return;

        gameObject.SetActive(savedState);

        Debug.Log(
            "Restored object: " +
            objectID +
            " = " +
            savedState
        );
    }
}
