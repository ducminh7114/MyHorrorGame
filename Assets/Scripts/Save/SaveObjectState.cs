using UnityEngine;

public static class SaveObjectState
{
    public static void SetActive(
        GameObject obj,
        bool active
    )
    {
        if (obj == null)
            return;

        obj.SetActive(active);

        PersistentObject persistent =
            obj.GetComponent<PersistentObject>();

        if (persistent != null)
        {
            GameSaveManager.Instance.SetObjectState(
                persistent.objectID,
                active
            );
        }
    }
}