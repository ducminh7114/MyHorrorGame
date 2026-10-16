
using UnityEngine;

public static class SaveObjectState
{
    public static void SetActive(GameObject obj, bool active)
    {
        if (obj == null)
            return;

        PersistentObject persistent =
            obj.GetComponent<PersistentObject>();

        if (persistent != null && GameSaveManager.Instance != null)
        {
            GameSaveManager.Instance.SetObjectState(
                persistent.objectID, active
            );
        }

        obj.SetActive(active);
    }
}